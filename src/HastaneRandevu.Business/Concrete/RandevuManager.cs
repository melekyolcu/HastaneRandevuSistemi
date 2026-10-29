using HastaneRandevu.Business.Abstract;
using HastaneRandevu.Business.Results;
using HastaneRandevu.DataAccess.Abstract;
using HastaneRandevu.Entities.Concrete;

namespace HastaneRandevu.Business.Concrete;

public class RandevuManager : IRandevuService
{
    private readonly IRandevuRepository _randevuRepository;
    private readonly IDoktorRepository _doktorRepository;
    private readonly IGenericRepository<Hasta> _hastaRepository;
    private readonly IGenericRepository<Hizmet> _hizmetRepository;

    // Gerekli repository'ler dışarıdan verilir (dependency injection)
    public RandevuManager(
        IRandevuRepository randevuRepository,
        IDoktorRepository doktorRepository,
        IGenericRepository<Hasta> hastaRepository,
        IGenericRepository<Hizmet> hizmetRepository)
    {
        _randevuRepository = randevuRepository;
        _doktorRepository = doktorRepository;
        _hastaRepository = hastaRepository;
        _hizmetRepository = hizmetRepository;
    }

    // ---------------- Hasta işlemleri ----------------

    public async Task<IslemSonucu<Randevu>> OlusturAsync(int hastaId, int doktorId, int hizmetId, DateTime baslangic, string? hastaNotu = null)
    {
        // Saniyeleri at, tarihi veritabanındaki sütun türüne uygun hale getir
        baslangic = DateTime.SpecifyKind(
            new DateTime(baslangic.Year, baslangic.Month, baslangic.Day, baslangic.Hour, baslangic.Minute, 0),
            DateTimeKind.Unspecified);

        if (baslangic <= DateTime.Now)
            return IslemSonucu<Randevu>.Hata("Geçmiş bir tarihe randevu alınamaz.");

        var hasta = await _hastaRepository.GetirAsync(hastaId);
        if (hasta is null)
            return IslemSonucu<Randevu>.Hata("Hasta bulunamadı.");

        var hizmet = await _hizmetRepository.GetirAsync(hizmetId);
        if (hizmet is null)
            return IslemSonucu<Randevu>.Hata("Seçilen hizmet bulunamadı.");

        var doktor = await _doktorRepository.DetayliGetirAsync(doktorId);
        if (doktor is null)
            return IslemSonucu<Randevu>.Hata("Doktor bulunamadı.");

        if (!doktor.HizmetVerebilirMi(hizmet))
            return IslemSonucu<Randevu>.Hata($"{doktor.Unvan} {doktor.AdSoyad} bu hizmeti vermiyor.");

        var bitis = baslangic.Add(hizmet.Sure);

        if (!doktor.UygunMu(baslangic, bitis))
            return IslemSonucu<Randevu>.Hata("Doktor seçilen saatte randevu kabul etmiyor.");

        if (await _randevuRepository.CakismaVarMiAsync(doktorId, baslangic, bitis))
            return IslemSonucu<Randevu>.Hata("Doktorun bu saatte başka bir randevusu var.");

        if (await _randevuRepository.HastaninCakismasiVarMiAsync(hastaId, baslangic, bitis))
            return IslemSonucu<Randevu>.Hata("Bu saatte zaten başka bir randevunuz var.");

        var randevu = new Randevu(hasta, doktor, hizmet, baslangic) { HastaNotu = hastaNotu };
        await _randevuRepository.EkleAsync(randevu);

        return IslemSonucu<Randevu>.Basari(randevu, "Randevu talebiniz alındı, doktor onayı bekleniyor.");
    }

    public async Task<IslemSonucu> IptalEtAsync(int randevuId, int hastaId)
    {
        var randevu = await _randevuRepository.GetirAsync(randevuId);

        // Hasta sadece kendi randevusunu iptal edebilir
        if (randevu is null || randevu.HastaId != hastaId)
            return IslemSonucu.Hata("Randevu bulunamadı.");

        if (randevu.BaslangicZamani <= DateTime.Now)
            return IslemSonucu.Hata("Zamanı geçmiş bir randevu iptal edilemez.");

        return await DurumIslemiYapAsync(randevu, r => r.IptalEt(), "Randevunuz iptal edildi.");
    }

    public Task<List<Randevu>> HastaninRandevulariAsync(int hastaId) =>
        _randevuRepository.HastaninRandevulariAsync(hastaId);

    // Randevu ekranında hastaya gösterilecek boş saatleri hesaplar
    public async Task<List<DateTime>> UygunSaatleriGetirAsync(int doktorId, int hizmetId, DateTime gun)
    {
        var bosSaatler = new List<DateTime>();

        var doktor = await _doktorRepository.DetayliGetirAsync(doktorId);
        var hizmet = await _hizmetRepository.GetirAsync(hizmetId);
        if (doktor is null || hizmet is null || !doktor.HizmetVerebilirMi(hizmet))
            return bosSaatler;

        var tarih = DateTime.SpecifyKind(gun.Date, DateTimeKind.Unspecified);
        var gununRandevulari = await _randevuRepository.DoktorunGunlukRandevulariAsync(doktorId, tarih);

        var gununDilimleri = doktor.UygunlukZamanlari
            .Where(u => u.AktifMi && u.Gun == tarih.DayOfWeek)
            .OrderBy(u => u.BaslangicSaati);

        // Her uygunluk dilimini hizmet süresi kadar parçalara böl, dolu olanları ele
        foreach (var dilim in gununDilimleri)
        {
            var slotBaslangic = tarih.Add(dilim.BaslangicSaati.ToTimeSpan());
            var dilimSonu = tarih.Add(dilim.BitisSaati.ToTimeSpan());

            while (slotBaslangic.Add(hizmet.Sure) <= dilimSonu)
            {
                var slotBitis = slotBaslangic.Add(hizmet.Sure);

                bool gelecekteMi = slotBaslangic > DateTime.Now;
                bool doluMu = gununRandevulari.Any(r => r.CakisiyorMu(slotBaslangic, slotBitis));

                if (gelecekteMi && !doluMu)
                    bosSaatler.Add(slotBaslangic);

                slotBaslangic = slotBitis;
            }
        }

        return bosSaatler;
    }

    // ---------------- Doktor işlemleri ----------------

    public async Task<IslemSonucu> OnaylaAsync(int randevuId, int doktorId)
    {
        var randevu = await DoktorunRandevusunuGetirAsync(randevuId, doktorId);
        if (randevu is null)
            return IslemSonucu.Hata("Randevu bulunamadı.");

        return await DurumIslemiYapAsync(randevu, r => r.Onayla(), "Randevu onaylandı.");
    }

    public async Task<IslemSonucu> ReddetAsync(int randevuId, int doktorId, string neden)
    {
        var randevu = await DoktorunRandevusunuGetirAsync(randevuId, doktorId);
        if (randevu is null)
            return IslemSonucu.Hata("Randevu bulunamadı.");

        return await DurumIslemiYapAsync(randevu, r => r.Reddet(neden), "Randevu reddedildi.");
    }

    public async Task<IslemSonucu> TamamlaAsync(int randevuId, int doktorId)
    {
        var randevu = await DoktorunRandevusunuGetirAsync(randevuId, doktorId);
        if (randevu is null)
            return IslemSonucu.Hata("Randevu bulunamadı.");

        if (randevu.BaslangicZamani > DateTime.Now)
            return IslemSonucu.Hata("Henüz zamanı gelmemiş bir randevu tamamlanamaz.");

        return await DurumIslemiYapAsync(randevu, r => r.Tamamla(), "Randevu tamamlandı olarak işaretlendi.");
    }

    public Task<List<Randevu>> OnayBekleyenlerAsync(int doktorId) =>
        _randevuRepository.OnayBekleyenlerAsync(doktorId);

    public Task<List<Randevu>> DoktorunGunlukRandevulariAsync(int doktorId, DateTime gun) =>
        _randevuRepository.DoktorunGunlukRandevulariAsync(doktorId, gun);

    // ---------------- Yardımcı metotlar ----------------

    // Doktor sadece kendine ait randevular üzerinde işlem yapabilir
    private async Task<Randevu?> DoktorunRandevusunuGetirAsync(int randevuId, int doktorId)
    {
        var randevu = await _randevuRepository.GetirAsync(randevuId);
        return randevu is not null && randevu.DoktorId == doktorId ? randevu : null;
    }

    // Randevu class'ındaki kurallar ihlal edilirse fırlatılan hatayı kullanıcıya gösterilecek mesaja çevirir
    private async Task<IslemSonucu> DurumIslemiYapAsync(Randevu randevu, Action<Randevu> islem, string basariMesaji)
    {
        try
        {
            islem(randevu);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            return IslemSonucu.Hata(ex.Message);
        }

        await _randevuRepository.GuncelleAsync(randevu);
        return IslemSonucu.Basari(basariMesaji);
    }
}