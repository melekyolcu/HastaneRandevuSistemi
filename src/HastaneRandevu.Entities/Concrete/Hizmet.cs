using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Entities.Concrete;


/// sunulan sağlık hizmeti muayne, tahlil vs.

public class Hizmet : BaseEntity
{
    private string _ad = string.Empty;
    private int _sureDakika;
    private decimal _ucret;

    public string Ad
    {
        get => _ad;
        set => _ad = BosOlamaz(value, "Hizmet adı");
    }

    public string Aciklama { get; set; } = string.Empty;
    public HizmetKategorisi Kategori { get; set; }

    public int SureDakika
    {
        get => _sureDakika;
        set
        {
            if (value <= 0 || value > 480)
                throw new ArgumentException("Hizmet süresi 1 ile 480 dakika arasında olmalıdır.");
            _sureDakika = value;
        }
    }

    public decimal Ucret
    {
        get => _ucret;
        set
        {
            if (value < 0)
                throw new ArgumentException("Ücret negatif olamaz.");
            _ucret = value;
        }
    }

    public int BolumId { get; set; }
    public Bolum Bolum { get; set; } = null!;

    // Bu hizmeti verebilen doktorlar (çoka çok ilişki)
    public ICollection<Doktor> Doktorlar { get; set; } = new List<Doktor>();

    // Randevu hesaplarında kullanacağım 
    public TimeSpan Sure => TimeSpan.FromMinutes(SureDakika);
}