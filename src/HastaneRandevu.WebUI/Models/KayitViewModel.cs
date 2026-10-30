using System.ComponentModel.DataAnnotations;

namespace HastaneRandevu.WebUI.Models;

public class KayitViewModel
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Ad")]
    public string Ad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Soyad")]
    public string Soyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "TC Kimlik No zorunludur.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.")]
    [Display(Name = "TC Kimlik No")]
    public string TcKimlikNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Doğum Tarihi")]
    public DateTime? DogumTarihi { get; set; }

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [Display(Name = "E-posta")]
    public string Eposta { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [Display(Name = "Telefon")]
    public string? Telefon { get; set; }

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Sifre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [Compare(nameof(Sifre), ErrorMessage = "Şifreler eşleşmiyor.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre (Tekrar)")]
    public string SifreTekrar { get; set; } = string.Empty;
}