using HastaneRandevu.DataAccess.Context;
using HastaneRandevu.Entities.Concrete;
using HastaneRandevu.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.DataAccess.Seed;


/// Geliştirme sırasında boş veritabanını örnek verilerle doldurur.
/// Veritabanında zaten kurum varsa hiçbir şey yapmaz.
public static class OrnekVeriYukleyici
{
    private static readonly DayOfWeek[] HaftaIci =
    {
        DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
        DayOfWeek.Thursday, DayOfWeek.Friday
    };

    public static async Task YukleAsync(HastaneRandevuContext context)
    {
        if (await context.Kurumlar.AnyAsync())
            return;

        // ---- Kurum ve şubeler ----
        var kurum = new Kurum
        {
            Ad = "Şifa Sağlık Grubu",
            Turu = KurumTuru.Hastane,
            Telefon = "0212 000 00 00",
            Eposta = "info@sifasaglik.ornek"
        };

        var merkez = new Sube { Ad = "Merkez Şube", Sehir = "İstanbul", Adres = "Örnek Mah. Sağlık Cad. No:1" };
        var anadolu = new Sube { Ad = "Anadolu Şube", Sehir = "İstanbul", Adres = "Örnek Mah. Şifa Sok. No:5" };
        kurum.Subeler.Add(merkez);
        kurum.Subeler.Add(anadolu);

        foreach (var sube in kurum.Subeler)
            CalismaSaatleriniEkle(sube);

        // ---- Bölümler ve hizmetler ----
        var dahiliye = BolumEkle(merkez, "Dahiliye", "İç hastalıkları");
        var kardiyoloji = BolumEkle(merkez, "Kardiyoloji", "Kalp ve damar hastalıkları");
        var radyoloji = BolumEkle(merkez, "Radyoloji", "Görüntüleme hizmetleri");
        var anadoluDahiliye = BolumEkle(anadolu, "Dahiliye", "İç hastalıkları");

        var genelMuayene = HizmetEkle(dahiliye, "Genel Muayene", HizmetKategorisi.Muayene, 15, 750m);
        var kontrolMuayenesi = HizmetEkle(dahiliye, "Kontrol Muayenesi", HizmetKategorisi.Muayene, 10, 400m);
        var kardiyolojiMuayenesi = HizmetEkle(kardiyoloji, "Kardiyoloji Muayenesi", HizmetKategorisi.Muayene, 20, 1200m);
        var ekg = HizmetEkle(kardiyoloji, "EKG", HizmetKategorisi.Diger, 15, 600m);
        var rontgen = HizmetEkle(radyoloji, "Akciğer Röntgeni", HizmetKategorisi.Radyoloji, 15, 500m);
        var mr = HizmetEkle(radyoloji, "MR Çekimi", HizmetKategorisi.Radyoloji, 45, 3500m);
        var anadoluMuayene = HizmetEkle(anadoluDahiliye, "Genel Muayene", HizmetKategorisi.Muayene, 15, 700m);
        var kanTahlili = HizmetEkle(anadoluDahiliye, "Kan Tahlili", HizmetKategorisi.Laboratuvar, 10, 350m);

        context.Kurumlar.Add(kurum);

        // ---- Doktorlar ----
        var drAyse = new Doktor
        {
            Ad = "Ayşe", Soyad = "Yılmaz", Eposta = "ayse.yilmaz@sifasaglik.ornek",
            Unvan = "Uzm. Dr.", UzmanlikAlani = "İç Hastalıkları",
            SicilNo = "D001", IseBaslamaTarihi = new DateTime(2018, 9, 1), Bolum = dahiliye
        };
        drAyse.Hizmetler.Add(genelMuayene);
        drAyse.Hizmetler.Add(kontrolMuayenesi);
        UygunlukEkle(drAyse, HaftaIci, 9, 12);
        UygunlukEkle(drAyse, HaftaIci, 13, 17);

        var drMehmet = new Doktor
        {
            Ad = "Mehmet", Soyad = "Kaya", Eposta = "mehmet.kaya@sifasaglik.ornek",
            Unvan = "Prof. Dr.", UzmanlikAlani = "Kardiyoloji",
            SicilNo = "D002", IseBaslamaTarihi = new DateTime(2010, 2, 15), Bolum = kardiyoloji
        };
        drMehmet.Hizmetler.Add(kardiyolojiMuayenesi);
        drMehmet.Hizmetler.Add(ekg);
        UygunlukEkle(drMehmet, new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday }, 9, 15);

        var drZeynep = new Doktor
        {
            Ad = "Zeynep", Soyad = "Demir", Eposta = "zeynep.demir@sifasaglik.ornek",
            Unvan = "Uzm. Dr.", UzmanlikAlani = "Radyoloji",
            SicilNo = "D003", IseBaslamaTarihi = new DateTime(2020, 6, 1), Bolum = radyoloji
        };
        drZeynep.Hizmetler.Add(rontgen);
        drZeynep.Hizmetler.Add(mr);
        UygunlukEkle(drZeynep, new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday }, 8, 16);

        var drCan = new Doktor
        {
            Ad = "Can", Soyad = "Öztürk", Eposta = "can.ozturk@sifasaglik.ornek",
            Unvan = "Uzm. Dr.", UzmanlikAlani = "İç Hastalıkları",
            SicilNo = "D004", IseBaslamaTarihi = new DateTime(2019, 3, 10), Bolum = anadoluDahiliye
        };
        drCan.Hizmetler.Add(anadoluMuayene);
        drCan.Hizmetler.Add(kanTahlili);
        UygunlukEkle(drCan, HaftaIci, 9, 17);

        // ---- Diğer kullanıcılar ----
        var hemsire = new SaglikPersoneli
        {
            Ad = "Elif", Soyad = "Şahin", Eposta = "elif.sahin@sifasaglik.ornek",
            Gorev = "Hemşire", SicilNo = "P001", IseBaslamaTarihi = new DateTime(2021, 1, 4)
        };

        var yonetici = new Yonetici
        {
            Ad = "Sistem", Soyad = "Yöneticisi", Eposta = "yonetici@sifasaglik.ornek"
        };

        var hasta = new Hasta
        {
            Ad = "Ahmet", Soyad = "Çelik", Eposta = "ahmet.celik@ornek.com",
            Telefon = "0555 000 00 00", TcKimlikNo = "10000000146",
            DogumTarihi = new DateTime(1990, 5, 12)
        };

        context.Kullanicilar.AddRange(drAyse, drMehmet, drZeynep, drCan, hemsire, yonetici, hasta);

        // ---- Örnek randevu: önümüzdeki pazartesi 10:00, onay bekliyor ----
        var pazartesi = SonrakiGun(DayOfWeek.Monday);
        var randevu = new Randevu(hasta, drAyse, genelMuayene, pazartesi.AddHours(10))
        {
            HastaNotu = "Son günlerde halsizlik var."
        };
        context.Randevular.Add(randevu);

        await context.SaveChangesAsync();
    }

    
    public const string VarsayilanSifre = "Sifre123";

    // ornek kullanicilara ortak sifre
    public static async Task VarsayilanSifreleriAyarlaAsync(HastaneRandevuContext context)
    {
        var sifresizler = await context.Kullanicilar
            .Where(k => k.SifreHash == "")
            .ToListAsync();

        if (sifresizler.Count == 0)
            return;

        foreach (var kullanici in sifresizler)
            kullanici.SifreBelirle(VarsayilanSifre);

        await context.SaveChangesAsync();
    }

    // ---- Yardımcı metotlar ----

    private static void CalismaSaatleriniEkle(Sube sube)
    {
        foreach (var gun in HaftaIci)
        {
            var saat = new CalismaSaati { Gun = gun };
            saat.SaatleriAyarla(new TimeOnly(8, 0), new TimeOnly(18, 0));
            sube.CalismaSaatleri.Add(saat);
        }

        var cumartesi = new CalismaSaati { Gun = DayOfWeek.Saturday };
        cumartesi.SaatleriAyarla(new TimeOnly(9, 0), new TimeOnly(13, 0));
        sube.CalismaSaatleri.Add(cumartesi);

        sube.CalismaSaatleri.Add(new CalismaSaati { Gun = DayOfWeek.Sunday, KapaliMi = true });
    }

    private static Bolum BolumEkle(Sube sube, string ad, string aciklama)
    {
        var bolum = new Bolum { Ad = ad, Aciklama = aciklama };
        sube.Bolumler.Add(bolum);
        return bolum;
    }

    private static Hizmet HizmetEkle(Bolum bolum, string ad, HizmetKategorisi kategori, int sureDakika, decimal ucret)
    {
        var hizmet = new Hizmet { Ad = ad, Kategori = kategori, SureDakika = sureDakika, Ucret = ucret };
        bolum.Hizmetler.Add(hizmet);
        return hizmet;
    }

    private static void UygunlukEkle(Doktor doktor, IEnumerable<DayOfWeek> gunler, int baslangicSaati, int bitisSaati)
    {
        foreach (var gun in gunler)
        {
            var dilim = new UygunlukZamani { Gun = gun };
            dilim.SaatleriAyarla(new TimeOnly(baslangicSaati, 0), new TimeOnly(bitisSaati, 0));
            doktor.UygunlukZamanlari.Add(dilim);
        }
    }

    private static DateTime SonrakiGun(DayOfWeek gun)
    {
        var bugun = DateTime.Today;
        int fark = ((int)gun - (int)bugun.DayOfWeek + 7) % 7;
        if (fark == 0) fark = 7;
        var tarih = bugun.AddDays(fark);
        return new DateTime(tarih.Year, tarih.Month, tarih.Day);
    }
}