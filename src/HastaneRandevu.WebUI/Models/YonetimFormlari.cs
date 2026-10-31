using System.ComponentModel.DataAnnotations;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.WebUI.Models;

public class KurumFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kurum adı zorunludur.")]
    [StringLength(150)]
    [Display(Name = "Kurum Adı")]
    public string Ad { get; set; } = string.Empty;

    [Display(Name = "Kurum Türü")]
    public KurumTuru Turu { get; set; } = KurumTuru.Hastane;

    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [Display(Name = "Telefon")]
    public string? Telefon { get; set; }

    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [Display(Name = "E-posta")]
    public string? Eposta { get; set; }
}

public class SubeFormViewModel
{
    public int Id { get; set; }
    public int KurumId { get; set; }
    public string? KurumAdi { get; set; }

    [Required(ErrorMessage = "Şube adı zorunludur.")]
    [StringLength(150)]
    [Display(Name = "Şube Adı")]
    public string Ad { get; set; } = string.Empty;

    [Display(Name = "Şehir")]
    public string? Sehir { get; set; }

    [Display(Name = "Adres")]
    public string? Adres { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [Display(Name = "Telefon")]
    public string? Telefon { get; set; }
}

public class BolumFormViewModel
{
    public int Id { get; set; }
    public int SubeId { get; set; }

    [Required(ErrorMessage = "Bölüm adı zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Bölüm Adı")]
    public string Ad { get; set; } = string.Empty;

    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }
}

public class HizmetFormViewModel
{
    public int Id { get; set; }
    public int BolumId { get; set; }
    public int SubeId { get; set; }
    public string? BolumAdi { get; set; }

    [Required(ErrorMessage = "Hizmet adı zorunludur.")]
    [StringLength(150)]
    [Display(Name = "Hizmet Adı")]
    public string Ad { get; set; } = string.Empty;

    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    [Display(Name = "Kategori")]
    public HizmetKategorisi Kategori { get; set; } = HizmetKategorisi.Muayene;

    [Range(1, 480, ErrorMessage = "Süre 1 ile 480 dakika arasında olmalıdır.")]
    [Display(Name = "Süre (dakika)")]
    public int SureDakika { get; set; } = 15;

    // Kuruşsuz tam TL; ondalıklı sayılarda Türkçe virgül/nokta karışıklığını önlemek için
    [Range(0, 1_000_000, ErrorMessage = "Geçerli bir ücret girin.")]
    [Display(Name = "Ücret (TL)")]
    public int Ucret { get; set; }
}