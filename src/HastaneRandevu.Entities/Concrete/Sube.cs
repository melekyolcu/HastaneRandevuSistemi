using HastaneRandevu.Entities.Abstract;

namespace HastaneRandevu.Entities.Concrete;

public class Sube : BaseEntity
{
    private string _ad = string.Empty;

    public string Ad
    {
        get => _ad;
        set => _ad = BosOlamaz(value, "Şube adı");
    }

    public string Sehir { get; set; } = string.Empty;
    public string Adres { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;

    // Şube hangi kuruma ait
    public int KurumId { get; set; }
    public Kurum Kurum { get; set; } = null!;

    public ICollection<Bolum> Bolumler { get; set; } = new List<Bolum>();
    public ICollection<CalismaSaati> CalismaSaatleri { get; set; } = new List<CalismaSaati>();

  
    /// Şube verilen tarih ve saatte açık mı?
    public bool AcikMi(DateTime zaman)
    {
        var gunlukSaat = CalismaSaatleri.FirstOrDefault(c => c.Gun == zaman.DayOfWeek);
        return gunlukSaat is not null && gunlukSaat.AcikMi(TimeOnly.FromDateTime(zaman));
    }
}