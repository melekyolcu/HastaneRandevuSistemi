using System.ComponentModel.DataAnnotations;
using HastaneRandevu.Entities.Concrete;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaneRandevu.WebUI.Models;

public class DoktorFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Ad")]
    public string Ad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Soyad")]
    public string Soyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [Display(Name = "E-posta")]
    public string Eposta { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [Display(Name = "Telefon")]
    public string? Telefon { get; set; }

    [Required(ErrorMessage = "Unvan seçin.")]
    [Display(Name = "Unvan")]
    public string Unvan { get; set; } = "Dr.";

    [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
    [Display(Name = "Uzmanlık Alanı")]
    public string UzmanlikAlani { get; set; } = string.Empty;

    [Display(Name = "Sicil No")]
    public string? SicilNo { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "İşe Başlama Tarihi")]
    public DateTime? IseBaslamaTarihi { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Bir bölüm seçin.")]
    [Display(Name = "Bölüm")]
    public int BolumId { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string? Sifre { get; set; }

    // Formdan gelmez, sadece açılır listeyi doldurmak için
    [ValidateNever]
    public List<SelectListItem> Bolumler { get; set; } = new();
}

public class PersonelFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Ad")]
    public string Ad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(50)]
    [Display(Name = "Soyad")]
    public string Soyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [Display(Name = "E-posta")]
    public string Eposta { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [Display(Name = "Telefon")]
    public string? Telefon { get; set; }

    [Required(ErrorMessage = "Görev zorunludur.")]
    [Display(Name = "Görev")]
    public string Gorev { get; set; } = string.Empty;

    [Display(Name = "Sicil No")]
    public string? SicilNo { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "İşe Başlama Tarihi")]
    public DateTime? IseBaslamaTarihi { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string? Sifre { get; set; }
}

public class DoktorDetayViewModel
{
    public Doktor Doktor { get; set; } = null!;
    public List<Hizmet> VerilebilecekHizmetler { get; set; } = new();
}