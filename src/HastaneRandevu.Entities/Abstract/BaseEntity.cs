namespace HastaneRandevu.Entities.Abstract;

/// Bütün varlıkların ortak özelliklerini tutan soyut temel class.

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? GuncellemeTarihi { get; set; }
    public bool AktifMi { get; set; } = true;
}