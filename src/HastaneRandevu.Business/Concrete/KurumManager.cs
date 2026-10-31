using HastaneRandevu.Business.Abstract;
using HastaneRandevu.Business.Results;
using HastaneRandevu.DataAccess.Abstract;
using HastaneRandevu.Entities.Concrete;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Business.Concrete;

public class KurumManager : IKurumService
{
    private readonly IKurumRepository _kurumRepository;
    private readonly ISubeRepository _subeRepository;
    private readonly IGenericRepository<Bolum> _bolumRepository;
    private readonly IGenericRepository<Hizmet> _hizmetRepository;
    private readonly IGenericRepository<CalismaSaati> _calismaSaatiRepository;
    private readonly IDoktorRepository _doktorRepository;
    private readonly IRandevuRepository _randevuRepository;

    public KurumManager(
        IKurumRepository kurumRepository,
        ISubeRepository subeRepository,
        IGenericRepository<Bolum> bolumRepository,
        IGenericRepository<Hizmet> hizmetRepository,
        IGenericRepository<CalismaSaati> calismaSaatiRepository,
        IDoktorRepository doktorRepository,
        IRandevuRepository randevuRepository)
    {
        _kurumRepository = kurumRepository;
        _subeRepository = subeRepository;
        _bolumRepository = bolumRepository;
        _hizmetRepository = hizmetRepository;
        _calismaSaatiRepository = calismaSaatiRepository;
        _doktorRepository = doktorRepository;
        _randevuRepository = randevuRepository;
    }

    // ---------------- Kurum ----------------

    public Task<List<Kurum>> KurumlariListeleAsync() => _kurumRepository.SubeleriyleListeleAsync();

    public Task<Kurum?> KurumGetirAsync(int id) => _kurumRepository.SubeleriyleGetirAsync(id);

    public async Task<IslemSonucu> KurumKaydetAsync(int id, string ad, KurumTuru turu, string? telefon, string? eposta)
    {
        try
        {
            if (id == 0)
            {
                var yeniKurum = new Kurum
                {
                    Ad = ad,
                    Turu = turu,
                    Telefon = telefon ?? string.Empty,
                    Eposta = eposta ?? string.Empty
                };
                await _kurumRepository.EkleAsync(yeniKurum);
                return IslemSonucu.Basari("Kurum eklendi.");
            }

            var kurum = await _kurumRepository.GetirAsync(id);
            if (kurum is null)
                return IslemSonucu.Hata("Kurum bulunamadı.");

            kurum.Ad = ad;
            kurum.Turu = turu;
            kurum.Telefon = telefon ?? string.Empty;
            kurum.Eposta = eposta ?? string.Empty;

            await _kurumRepository.GuncelleAsync(kurum);
            return IslemSonucu.Basari("Kurum güncellendi.");
        }
        catch (ArgumentException ex)
        {
            return IslemSonucu.Hata(ex.Message);
        }
    }

    public async Task<IslemSonucu> KurumSilAsync(int id)
    {
        var kurum = await _kurumRepository.GetirAsync(id);
        if (kurum is null)
            return IslemSonucu.Hata("Kurum bulunamadı.");

        if (await _subeRepository.VarMiAsync(s => s.KurumId == id))
            return IslemSonucu.Hata("Bu kuruma bağlı şubeler var. Önce şubeleri silin.");

        await _kurumRepository.SilAsync(kurum);
        return IslemSonucu.Basari("Kurum silindi.");
    }

    // ---------------- Şube ----------------

    public Task<Sube?> SubeDetayliGetirAsync(int id) => _subeRepository.DetayliGetirAsync(id);

    public async Task<IslemSonucu> SubeKaydetAsync(int id, int kurumId, string ad, string? sehir, string? adres, string? telefon)
    {
        try
        {
            if (id == 0)
            {
                if (await _kurumRepository.GetirAsync(kurumId) is null)
                    return IslemSonucu.Hata("Kurum bulunamadı.");

                var yeniSube = new Sube
                {
                    KurumId = kurumId,
                    Ad = ad,
                    Sehir = sehir ?? string.Empty,
                    Adres = adres ?? string.Empty,
                    Telefon = telefon ?? string.Empty
                };
                VarsayilanCalismaSaatleriniEkle(yeniSube);

                await _subeRepository.EkleAsync(yeniSube);
                return IslemSonucu.Basari("Şube eklendi. Hafta içi 08:00-18:00 çalışma saatleri atandı, şube sayfasından değiştirebilirsiniz.");
            }

            var sube = await _subeRepository.GetirAsync(id);
            if (sube is null)
                return IslemSonucu.Hata("Şube bulunamadı.");

            sube.Ad = ad;
            sube.Sehir = sehir ?? string.Empty;
            sube.Adres = adres ?? string.Empty;
            sube.Telefon = telefon ?? string.Empty;

            await _subeRepository.GuncelleAsync(sube);
            return IslemSonucu.Basari("Şube güncellendi.");
        }
        catch (ArgumentException ex)
        {
            return IslemSonucu.Hata(ex.Message);
        }
    }

    public async Task<IslemSonucu> SubeSilAsync(int id)
    {
        var sube = await _subeRepository.GetirAsync(id);
        if (sube is null)
            return IslemSonucu.Hata("Şube bulunamadı.");

        if (await _bolumRepository.VarMiAsync(b => b.SubeId == id))
            return IslemSonucu.Hata("Bu şubeye bağlı bölümler var. Önce bölümleri silin.");

        await _subeRepository.SilAsync(sube);
        return IslemSonucu.Basari("Şube silindi.");
    }

    public async Task<IslemSonucu> CalismaSaatiKaydetAsync(int subeId, DayOfWeek gun, bool kapaliMi, TimeOnly acilis, TimeOnly kapanis)
    {
        var kayit = (await _calismaSaatiRepository.ListeleAsync(c => c.SubeId == subeId && c.Gun == gun))
            .FirstOrDefault();

        bool yeniKayit = kayit is null;
        kayit ??= new CalismaSaati { SubeId = subeId, Gun = gun };

        try
        {
            if (kapaliMi)
                kayit.KapaliMi = true;
            else
                kayit.SaatleriAyarla(acilis, kapanis);
        }
        catch (ArgumentException ex)
        {
            return IslemSonucu.Hata(ex.Message);
        }

        if (yeniKayit)
            await _calismaSaatiRepository.EkleAsync(kayit);
        else
            await _calismaSaatiRepository.GuncelleAsync(kayit);

        return IslemSonucu.Basari("Çalışma saati kaydedildi.");
    }

    // ---------------- Bölüm ----------------

    public Task<Bolum?> BolumGetirAsync(int id) => _bolumRepository.GetirAsync(id);

    public async Task<IslemSonucu> BolumKaydetAsync(int id, int subeId, string ad, string? aciklama)
    {
        try
        {
            if (id == 0)
            {
                if (await _subeRepository.GetirAsync(subeId) is null)
                    return IslemSonucu.Hata("Şube bulunamadı.");

                await _bolumRepository.EkleAsync(new Bolum
                {
                    SubeId = subeId,
                    Ad = ad,
                    Aciklama = aciklama ?? string.Empty
                });
                return IslemSonucu.Basari("Bölüm eklendi.");
            }

            var bolum = await _bolumRepository.GetirAsync(id);
            if (bolum is null)
                return IslemSonucu.Hata("Bölüm bulunamadı.");

            bolum.Ad = ad;
            bolum.Aciklama = aciklama ?? string.Empty;

            await _bolumRepository.GuncelleAsync(bolum);
            return IslemSonucu.Basari("Bölüm güncellendi.");
        }
        catch (ArgumentException ex)
        {
            return IslemSonucu.Hata(ex.Message);
        }
    }

    public async Task<IslemSonucu> BolumSilAsync(int id)
    {
        var bolum = await _bolumRepository.GetirAsync(id);
        if (bolum is null)
            return IslemSonucu.Hata("Bölüm bulunamadı.");

        if (await _hizmetRepository.VarMiAsync(h => h.BolumId == id))
            return IslemSonucu.Hata("Bu bölüme bağlı hizmetler var. Önce hizmetleri silin.");

        if (await _doktorRepository.VarMiAsync(d => d.BolumId == id))
            return IslemSonucu.Hata("Bu bölümde çalışan doktorlar var. Önce doktorları başka bölüme taşıyın.");

        await _bolumRepository.SilAsync(bolum);
        return IslemSonucu.Basari("Bölüm silindi.");
    }

    // ---------------- Hizmet ----------------

    public Task<Hizmet?> HizmetGetirAsync(int id) => _hizmetRepository.GetirAsync(id);

    public async Task<IslemSonucu> HizmetKaydetAsync(int id, int bolumId, string ad, string? aciklama,
        HizmetKategorisi kategori, int sureDakika, decimal ucret)
    {
        try
        {
            if (id == 0)
            {
                if (await _bolumRepository.GetirAsync(bolumId) is null)
                    return IslemSonucu.Hata("Bölüm bulunamadı.");

                await _hizmetRepository.EkleAsync(new Hizmet
                {
                    BolumId = bolumId,
                    Ad = ad,
                    Aciklama = aciklama ?? string.Empty,
                    Kategori = kategori,
                    SureDakika = sureDakika,
                    Ucret = ucret
                });
                return IslemSonucu.Basari("Hizmet eklendi.");
            }

            var hizmet = await _hizmetRepository.GetirAsync(id);
            if (hizmet is null)
                return IslemSonucu.Hata("Hizmet bulunamadı.");

            hizmet.Ad = ad;
            hizmet.Aciklama = aciklama ?? string.Empty;
            hizmet.Kategori = kategori;
            hizmet.SureDakika = sureDakika;
            hizmet.Ucret = ucret;

            // Alınmış randevular etkilenmez; onlar alındıkları andaki süre ve ücreti saklıyor
            await _hizmetRepository.GuncelleAsync(hizmet);
            return IslemSonucu.Basari("Hizmet güncellendi.");
        }
        catch (ArgumentException ex)
        {
            return IslemSonucu.Hata(ex.Message);
        }
    }

    public async Task<IslemSonucu> HizmetSilAsync(int id)
    {
        var hizmet = await _hizmetRepository.GetirAsync(id);
        if (hizmet is null)
            return IslemSonucu.Hata("Hizmet bulunamadı.");

        var simdi = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
        bool ileriTarihliRandevuVar = await _randevuRepository.VarMiAsync(r =>
            r.HizmetId == id
            && r.BaslangicZamani > simdi
            && (r.Durum == RandevuDurumu.Beklemede || r.Durum == RandevuDurumu.Onaylandi));

        if (ileriTarihliRandevuVar)
            return IslemSonucu.Hata("Bu hizmet için ileri tarihli randevular var, silinemez.");

        await _hizmetRepository.SilAsync(hizmet);
        return IslemSonucu.Basari("Hizmet silindi.");
    }

    // ---------------- Yardımcı metotlar ----------------

    // Yeni şubeye hafta içi 08:00-18:00, hafta sonu kapalı varsayılan saatler atanır
    private static void VarsayilanCalismaSaatleriniEkle(Sube sube)
    {
        foreach (var gun in Enum.GetValues<DayOfWeek>())
        {
            var saat = new CalismaSaati { Gun = gun };

            if (gun is DayOfWeek.Saturday or DayOfWeek.Sunday)
                saat.KapaliMi = true;
            else
                saat.SaatleriAyarla(new TimeOnly(8, 0), new TimeOnly(18, 0));

            sube.CalismaSaatleri.Add(saat);
        }
    }
}