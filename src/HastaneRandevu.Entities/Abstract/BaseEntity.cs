namespace HastaneRandevu.Entities.Abstract;

/// Tüm varlıkların ortak özelliklerini tutan soyut temel sınıf.



public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    public DateTime? GuncellemeTarihi { get; set; }
    public bool AktifMi { get; set; } = true;

   
    /// ortak kullanılan doğrulama yardımcısı.
    /// protected: sadece bu sınıf ve ondan türeyen sınıflar erişebilir.
    
    protected static string BosOlamaz(string? deger, string alanAdi)
    {
        if (string.IsNullOrWhiteSpace(deger))
            throw new ArgumentException($"{alanAdi} boş olamaz.");
        return deger.Trim();
    }
}