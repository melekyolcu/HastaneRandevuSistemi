using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Entities.Concrete;

/// Hastane veya klinik. Tek şubeli kurum, tek şubesi olan kurum olarak tanımlanır.

public class Kurum : BaseEntity
{
    private string _ad = string.Empty;

    public string Ad
    {
        get => _ad;
        set => _ad = BosOlamaz(value, "Kurum adı");
    }

    public KurumTuru Turu { get; set; }
    public string Telefon { get; set; } = string.Empty;
    public string Eposta { get; set; } = string.Empty;

    // Bir kurumun birden çok şubesi olabilir (1'e çok ilişki)
    public ICollection<Sube> Subeler { get; set; } = new List<Sube>();

    public bool CokSubeliMi => Subeler.Count > 1;
}