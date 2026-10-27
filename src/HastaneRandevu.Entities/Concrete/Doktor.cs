using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Entities.Concrete;

public class Doktor : Calisan
{
    public string Unvan { get; set; } = "Dr.";  
    public string UzmanlikAlani { get; set; } = string.Empty;

    // Doktorun çalıştığı bölüm (bölüm üzerinden şubeye ve kuruma ulaşılır)
    public int BolumId { get; set; }
    public Bolum Bolum { get; set; } = null!;

    // Doktorun verebildiği hizmetler (çoka çok ilişki)
    public ICollection<Hizmet> Hizmetler { get; set; } = new List<Hizmet>();

    public ICollection<UygunlukZamani> UygunlukZamanlari { get; set; } = new List<UygunlukZamani>();
    public ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();

    public bool HizmetVerebilirMi(Hizmet hizmet) => Hizmetler.Any(h => h.Id == hizmet.Id);

    public bool UygunMu(DateTime baslangic, DateTime bitis) =>
        UygunlukZamanlari.Any(u => u.Kapsar(baslangic, bitis));
    public override KullaniciRolu Rol => KullaniciRolu.Doktor;

    public override string BilgiGetir() => $"{Unvan} {AdSoyad} - {UzmanlikAlani}";
}