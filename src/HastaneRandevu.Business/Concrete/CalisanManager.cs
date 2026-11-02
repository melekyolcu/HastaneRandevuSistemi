using HastaneRandevu.Business.Abstract;
using HastaneRandevu.Business.Dtos;
using HastaneRandevu.Business.Results;
using HastaneRandevu.DataAccess.Abstract;
using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Concrete;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Business.Concrete;

public class CalisanManager : ICalisanService
{
    private readonly IDoktorRepository _doktorRepository;
    private readonly IGenericRepository<SaglikPersoneli> _personelRepository;
    private readonly IGenericRepository<Kullanici> _kullaniciRepository;
    private readonly IGenericRepository<Hizmet> _hizmetRepository;
    private readonly IGenericRepository<UygunlukZamani> _uygunlukRepository;
    private readonly IBolumRepository _bolumRepository;
    private readonly IRandevuRepository _randevuRepository;

    public CalisanManager(
        IDoktorRepository doktorRepository,
        IGenericRepository<SaglikPersoneli> personelRepository,
        IGenericRepository<Kullanici> kullaniciRepository,
        IGenericRepository<Hizmet> hizmetRepository,
        IGenericRepository<UygunlukZamani> uygunlukRepository,
        IBolumRepository bolumRepository,
        IRandevuRepository randevuRepository)
    {
        _doktorRepository = doktorRepository;
        _personelRepository = personelRepository;
        _kullaniciRepository = kullaniciRepository;
        _hizmetRepository = hizmetRepository;
        _uygunlukRepository = uygunlukRepository;
        _bolumRepository = bolumRepository;
        _randevuRepository = randevuRepository;
    }

    // ---------------- Doktor ----------------

    public Task<List<Doktor>> DoktorlariListeleAsync() => _doktorRepository.TumunuDetayliListeleAsync();

    public Task<Doktor?> DoktorGetirAsync(int id) => _doktorRepository.DetayliGetirAsync(id);

    public async Task<IslemSonucu> DoktorKaydetAsync(DoktorBilgisi bilgi)
    {
        var eposta = EpostaDuzenle(bilgi.Eposta);

        if (await _kullaniciRepository.VarMiAsync(k => k.Eposta == eposta && k.Id != bilgi.Id))
            return IslemSonucu.Hata("Bu e-posta adresi başka bir kullanıcıya ait.");

        if (await _bolumRepository.GetirAsync(bilgi.BolumId) is null)
            return IslemSonucu.Hata("Seçilen bölüm bulunamadı.");

        try
        {
            Doktor? doktor;

            if (bilgi.Id == 0)
            {
                if (string.IsNullOrWhiteSpace(bilgi.Sifre))
                    return IslemSonucu.Hata("Yeni doktor için şifre belirlenmelidir.");

                doktor = new Doktor();
            }
            else
            {
                doktor = await _doktorRepository.DetayliGetirAsync(bilgi.Id);
                if (doktor is null)
                    return IslemSonucu.Hata("Doktor bulunamadı.");

                // Bölüm değişirse eski bölümün hizmetleri doktordan kaldırılır
                if (doktor.BolumId != bilgi.BolumId)
                    doktor.Hizmetler.Clear();
            }

            doktor.Ad = bilgi.Ad;
            doktor.Soyad = bilgi.Soyad;
            doktor.Eposta = eposta;
            doktor.Telefon = bilgi.Telefon ?? string.Empty;
            doktor.Unvan = bilgi.Unvan;
            doktor.UzmanlikAlani = bilgi.UzmanlikAlani;
            doktor.SicilNo = bilgi.SicilNo ?? string.Empty;
            doktor.IseBaslamaTarihi = (bilgi.IseBaslamaTarihi ?? DateTime.Today).Date;
            doktor.BolumId = bilgi.BolumId;

            // Düzenlemede şifre alanı boş bırakılırsa mevcut şifre korunur
            if (!string.IsNullOrWhiteSpace(bilgi.Sifre))
                doktor.SifreBelirle(bilgi.Sifre);

            if (bilgi.Id == 0)
            {
                await _doktorRepository.EkleAsync(doktor);
                return IslemSonucu.Basari("Doktor eklendi. Hizmetlerini ve uygunluk saatlerini tanımlamayı unutmayın.");
            }

            await _doktorRepository.GuncelleAsync(doktor);
            return IslemSonucu.Basari("Doktor bilgileri güncellendi.");
        }
        catch (ArgumentException ex)
        {
            return IslemSonucu.Hata(ex.Message);
        }
    }

    public async Task<IslemSonucu> DoktorSilAsync(int id)
    {
        var doktor = await _doktorRepository.GetirAsync(id);
        if (doktor is null)
            return IslemSonucu.Hata("Doktor bulunamadı.");

        var simdi = Simdi();
        bool ileriTarihliRandevuVar = await _randevuRepository.VarMiAsync(r =>
            r.DoktorId == id
            && r.BaslangicZamani > simdi
            && (r.Durum == RandevuDurumu.Beklemede || r.Durum == RandevuDurumu.Onaylandi));

        if (ileriTarihliRandevuVar)
            return IslemSonucu.Hata("Doktorun ileri tarihli randevuları var. Önce bu randevuların iptal edilmesi gerekir.");

        await _doktorRepository.SilAsync(doktor);
        return IslemSonucu.Basari("Doktor silindi.");
    }

    // Doktor sadece kendi bölümündeki hizmetleri verebilir
    public async Task<List<Hizmet>> DoktorunVerebilecegiHizmetlerAsync(int doktorId)
    {
        var doktor = await _doktorRepository.GetirAsync(doktorId);
        if (doktor is null)
            return new List<Hizmet>();

        return (await _hizmetRepository.ListeleAsync(h => h.BolumId == doktor.BolumId))
            .OrderBy(h => h.Ad)
            .ToList();
    }

    public async Task<IslemSonucu> DoktorHizmetleriniGuncelleAsync(int doktorId, IEnumerable<int> hizmetIdleri)
    {
        var doktor = await _doktorRepository.DetayliGetirAsync(doktorId);
        if (doktor is null)
            return IslemSonucu.Hata("Doktor bulunamadı.");

        var idler = hizmetIdleri.Distinct().ToList();
        var secilenler = await _hizmetRepository.ListeleAsync(h => idler.Contains(h.Id) && h.BolumId == doktor.BolumId);

        doktor.Hizmetler.Clear();
        foreach (var hizmet in secilenler)
            doktor.Hizmetler.Add(hizmet);

        await _doktorRepository.GuncelleAsync(doktor);
        return IslemSonucu.Basari($"Doktora {secilenler.Count} hizmet atandı.");
    }

    // ---------------- Uygunluk ----------------

    public async Task<IslemSonucu> UygunlukEkleAsync(int doktorId, DayOfWeek gun, TimeOnly baslangic, TimeOnly bitis)
    {
        var doktor = await _doktorRepository.DetayliGetirAsync(doktorId);
        if (doktor is null)
            return IslemSonucu.Hata("Doktor bulunamadı.");

        if (bitis <= baslangic)
            return IslemSonucu.Hata("Bitiş saati, başlangıç saatinden sonra olmalıdır.");

        // Doktor, şubenin kapalı olduğu gün ve saatlerde randevu kabul edemez
        var subeSaati = doktor.Bolum.Sube.CalismaSaatleri.FirstOrDefault(c => c.Gun == gun);
        if (subeSaati is null || subeSaati.KapaliMi)
            return IslemSonucu.Hata("Şube bu gün kapalı.");

        if (baslangic < subeSaati.AcilisSaati || bitis > subeSaati.KapanisSaati)
            return IslemSonucu.Hata(
                $"Uygunluk saatleri şubenin çalışma saatleri ({subeSaati.AcilisSaati:HH:mm} - {subeSaati.KapanisSaati:HH:mm}) içinde olmalıdır.");

        // Aynı gündeki diğer dilimlerle üst üste binmemeli
        bool cakisiyor = doktor.UygunlukZamanlari.Any(u =>
            u.Gun == gun && baslangic < u.BitisSaati && u.BaslangicSaati < bitis);

        if (cakisiyor)
            return IslemSonucu.Hata("Bu saatler doktorun o günkü başka bir uygunluk dilimiyle çakışıyor.");

        var dilim = new UygunlukZamani { DoktorId = doktorId, Gun = gun };
        dilim.SaatleriAyarla(baslangic, bitis);

        await _uygunlukRepository.EkleAsync(dilim);
        return IslemSonucu.Basari("Uygunluk saati eklendi.");
    }

    public async Task<IslemSonucu> UygunlukSilAsync(int uygunlukId, int doktorId)
    {
        var dilim = await _uygunlukRepository.GetirAsync(uygunlukId);
        if (dilim is null || dilim.DoktorId != doktorId)
            return IslemSonucu.Hata("Uygunluk kaydı bulunamadı.");

        await _uygunlukRepository.SilAsync(dilim);
        return IslemSonucu.Basari("Uygunluk saati silindi. Önceden alınmış randevular etkilenmez.");
    }

    // ---------------- Sağlık personeli ----------------

    public async Task<List<SaglikPersoneli>> PersonelListeleAsync() =>
        (await _personelRepository.ListeleAsync())
            .OrderBy(p => p.Ad).ThenBy(p => p.Soyad)
            .ToList();

    public Task<SaglikPersoneli?> PersonelGetirAsync(int id) => _personelRepository.GetirAsync(id);

    public async Task<IslemSonucu> PersonelKaydetAsync(PersonelBilgisi bilgi)
    {
        var eposta = EpostaDuzenle(bilgi.Eposta);

        if (await _kullaniciRepository.VarMiAsync(k => k.Eposta == eposta && k.Id != bilgi.Id))
            return IslemSonucu.Hata("Bu e-posta adresi başka bir kullanıcıya ait.");

        try
        {
            SaglikPersoneli? personel;

            if (bilgi.Id == 0)
            {
                if (string.IsNullOrWhiteSpace(bilgi.Sifre))
                    return IslemSonucu.Hata("Yeni personel için şifre belirlenmelidir.");

                personel = new SaglikPersoneli();
            }
            else
            {
                personel = await _personelRepository.GetirAsync(bilgi.Id);
                if (personel is null)
                    return IslemSonucu.Hata("Personel bulunamadı.");
            }

            personel.Ad = bilgi.Ad;
            personel.Soyad = bilgi.Soyad;
            personel.Eposta = eposta;
            personel.Telefon = bilgi.Telefon ?? string.Empty;
            personel.Gorev = bilgi.Gorev;
            personel.SicilNo = bilgi.SicilNo ?? string.Empty;
            personel.IseBaslamaTarihi = (bilgi.IseBaslamaTarihi ?? DateTime.Today).Date;

            if (!string.IsNullOrWhiteSpace(bilgi.Sifre))
                personel.SifreBelirle(bilgi.Sifre);

            if (bilgi.Id == 0)
            {
                await _personelRepository.EkleAsync(personel);
                return IslemSonucu.Basari("Personel eklendi.");
            }

            await _personelRepository.GuncelleAsync(personel);
            return IslemSonucu.Basari("Personel bilgileri güncellendi.");
        }
        catch (ArgumentException ex)
        {
            return IslemSonucu.Hata(ex.Message);
        }
    }

    public async Task<IslemSonucu> PersonelSilAsync(int id)
    {
        var personel = await _personelRepository.GetirAsync(id);
        if (personel is null)
            return IslemSonucu.Hata("Personel bulunamadı.");

        await _personelRepository.SilAsync(personel);
        return IslemSonucu.Basari("Personel silindi.");
    }

    // ---------------- Diğer ----------------

    public Task<List<Bolum>> BolumleriListeleAsync() => _bolumRepository.SubeVeKurumlaListeleAsync();

    private static string EpostaDuzenle(string? eposta) =>
        (eposta ?? string.Empty).Trim().ToLowerInvariant();

    private static DateTime Simdi() =>
        DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
}