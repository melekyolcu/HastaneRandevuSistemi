using HastaneRandevu.Entities.Abstract;

namespace HastaneRandevu.Entities.Concrete;


/// Şube içindeki bölüm (Kardiyoloji vs vs.)

public class Bolum : BaseEntity
{
    private string _ad = string.Empty;

    public string Ad
    {
        get => _ad;
        set => _ad = BosOlamaz(value, "Bölüm adı");
    }

    public string Aciklama { get; set; } = string.Empty;

    public int SubeId { get; set; }
    public Sube Sube { get; set; } = null!;

    public ICollection<Hizmet> Hizmetler { get; set; } = new List<Hizmet>();
}