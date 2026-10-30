using HastaneRandevu.Entities.Enums;
using System.Security.Cryptography;

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
        public string SifreHash { get; private set; } = string.Empty;

    private const int HashTekrarSayisi = 100_000;

    // Şifre düz metin olarak saklanmaz rastgele bir salt ile birlikte hash tutulur
    public void SifreBelirle(string sifre)
    {
        if (string.IsNullOrWhiteSpace(sifre) || sifre.Length < 6)
            throw new ArgumentException("Şifre en az 6 karakter olmalıdır.");

        var tuz = RandomNumberGenerator.GetBytes(16);
        var ozet = Rfc2898DeriveBytes.Pbkdf2(sifre, tuz, HashTekrarSayisi, HashAlgorithmName.SHA256, 32);

        SifreHash = $"{Convert.ToBase64String(tuz)}.{Convert.ToBase64String(ozet)}";
    }

    // Girilen şifre aynı saltla tekrar hashlenir ve kayıtlı hashle karşılaştırılır
    public bool SifreDogruMu(string sifre)
    {
        var parcalar = SifreHash.Split('.');
        if (parcalar.Length != 2 || string.IsNullOrEmpty(sifre))
            return false;

        var tuz = Convert.FromBase64String(parcalar[0]);
        var kayitliOzet = Convert.FromBase64String(parcalar[1]);
        var girilenOzet = Rfc2898DeriveBytes.Pbkdf2(sifre, tuz, HashTekrarSayisi, HashAlgorithmName.SHA256, 32);

        return CryptographicOperations.FixedTimeEquals(kayitliOzet, girilenOzet);
    }

    // Hesaplanan özellik: veritabanında tutulmaz
    public string AdSoyad => $"{Ad} {Soyad}";

    // Çok biçimlilik: her alt sınıf kendi rolünü söylemek ZORUNDA
    public abstract KullaniciRolu Rol { get; }

    // Çok biçimlilik: alt sınıflar isterse ezebilir (override)
    public virtual string BilgiGetir() => $"{AdSoyad} ({Rol})";
}