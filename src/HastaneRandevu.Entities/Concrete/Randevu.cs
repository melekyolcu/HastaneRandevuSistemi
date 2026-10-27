using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Entities.Concrete;

public class Randevu : BaseEntity
{
    // EF Core'un veritabanından okurken nesne oluşturabilmesi için gerekli
    protected Randevu() { }

    
    /// Yeni randevu sadece bu kurucu metotla oluşturulabilir;
    /// böylece hasta, doktor ve hizmeti olmayan "yarım" bir randevu var olamaz.
   
    public Randevu(Hasta hasta, Doktor doktor, Hizmet hizmet, DateTime baslangic)
    {
        ArgumentNullException.ThrowIfNull(hasta);
        ArgumentNullException.ThrowIfNull(doktor);
        ArgumentNullException.ThrowIfNull(hizmet);

        Hasta = hasta;
        HastaId = hasta.Id;
        Doktor = doktor;
        DoktorId = doktor.Id;
        Hizmet = hizmet;
        HizmetId = hizmet.Id;
        BaslangicZamani = baslangic;

        // Hizmetin o anki süresi ve ücreti randevuya kopyalanır.
        // Hizmetin fiyatı sonradan değişse bile alınmış randevu etkilenmez.
        SureDakika = hizmet.SureDakika;
        Ucret = hizmet.Ucret;

        Durum = RandevuDurumu.Beklemede;
    }

    public int HastaId { get; private set; }
    public Hasta Hasta { get; private set; } = null!;

    public int DoktorId { get; private set; }
    public Doktor Doktor { get; private set; } = null!;

    public int HizmetId { get; private set; }
    public Hizmet Hizmet { get; private set; } = null!;

    public DateTime BaslangicZamani { get; private set; }
    public int SureDakika { get; private set; }
    public decimal Ucret { get; private set; }

    public RandevuDurumu Durum { get; private set; }
    public string? HastaNotu { get; set; }
    public string? RedNedeni { get; private set; }

    public DateTime BitisZamani => BaslangicZamani.AddMinutes(SureDakika);

    // Reddedilmiş veya iptal edilmiş randevu, o saati başkasına açar
    public bool SlotuMesgulEdiyorMu =>
        Durum is RandevuDurumu.Beklemede or RandevuDurumu.Onaylandi;

   
    /// verilen zaman aralığı randevuyla çakışıyor mu
  
    public bool CakisiyorMu(DateTime digerBaslangic, DateTime digerBitis) =>
        SlotuMesgulEdiyorMu && BaslangicZamani < digerBitis && digerBaslangic < BitisZamani;

    //  Onay mekanizması: durum sadece bu metotlarla, kurallara uygun şekilde değişir

    public void Onayla()
    {
        DurumKontrolEt(RandevuDurumu.Beklemede);
        DurumuDegistir(RandevuDurumu.Onaylandi);
    }

    public void Reddet(string neden)
    {
        DurumKontrolEt(RandevuDurumu.Beklemede);
        RedNedeni = BosOlamaz(neden, "Red nedeni");
        DurumuDegistir(RandevuDurumu.Reddedildi);
    }

    public void IptalEt()
    {
        if (!SlotuMesgulEdiyorMu)
            throw new InvalidOperationException("Sadece bekleyen veya onaylanmış randevular iptal edilebilir.");
        DurumuDegistir(RandevuDurumu.IptalEdildi);
    }

    public void Tamamla()
    {
        DurumKontrolEt(RandevuDurumu.Onaylandi);
        DurumuDegistir(RandevuDurumu.Tamamlandi);
    }

    private void DurumKontrolEt(RandevuDurumu beklenenDurum)
    {
        if (Durum != beklenenDurum)
            throw new InvalidOperationException(
                $"Bu işlem için randevunun '{beklenenDurum}' durumunda olması gerekir. Mevcut durum: {Durum}");
    }

    private void DurumuDegistir(RandevuDurumu yeniDurum)
    {
        Durum = yeniDurum;
        GuncellemeTarihi = DateTime.UtcNow;
    }
}