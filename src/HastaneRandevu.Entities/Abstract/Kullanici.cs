using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Entities.Abstract;


/// Sistemdeki tüm kullanıcıların soyut temel sınıfı.
/// Doğrudan "new Kullanici()" yapılamaz; Hasta, Doktor vb. üzerinden kullanılmalı.

public abstract class Kullanici : BaseEntity
{
    // Kapsülleme: alanlar private, dışarıya kontrollü property ile açılıyor
    private string _ad = string.Empty;
    private string _soyad = string.Empty;
    private string _eposta = string.Empty;

    public string Ad
    {
        get => _ad;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ad boş olamaz.");
            _ad = value.Trim();
        }
    }

    public string Soyad
    {
        get => _soyad;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Soyad boş olamaz.");
            _soyad = value.Trim();
        }
    }

    public string Eposta
    {
        get => _eposta;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
                throw new ArgumentException("Geçerli bir e-posta adresi giriniz.");
            _eposta = value.Trim().ToLowerInvariant();
        }
    }

    public string Telefon { get; set; } = string.Empty;
    public string SifreHash { get; set; } = string.Empty;

    // Hesaplanan özellik: veritabanında tutulmaz
    public string AdSoyad => $"{Ad} {Soyad}";

    // Çok biçimlilik: her alt sınıf kendi rolünü söylemek ZORUNDA
    public abstract KullaniciRolu Rol { get; }

    // Çok biçimlilik: alt sınıflar isterse ezebilir (override)
    public virtual string BilgiGetir() => $"{AdSoyad} ({Rol})";
}