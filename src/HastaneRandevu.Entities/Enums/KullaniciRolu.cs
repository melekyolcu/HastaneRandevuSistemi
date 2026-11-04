using System.ComponentModel.DataAnnotations;

namespace HastaneRandevu.Entities.Enums;

public enum KullaniciRolu
{
    Hasta = 1,
    Doktor = 2,
    
    [Display(Name = "Sağlık Personeli")]
    SaglikPersoneli = 3,

    [Display(Name = "Yönetici")]
    Yonetici = 4
}