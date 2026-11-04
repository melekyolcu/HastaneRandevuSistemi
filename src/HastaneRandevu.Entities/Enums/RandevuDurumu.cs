using System.ComponentModel.DataAnnotations;

namespace HastaneRandevu.Entities.Enums;

public enum RandevuDurumu
{
    [Display(Name = "Onay Bekliyor")]
    Beklemede = 1,

    [Display(Name = "Onaylandı")]
    Onaylandi = 2,

    [Display(Name = "Reddedildi")]
    Reddedildi = 3,

    [Display(Name = "İptal Edildi")]
    IptalEdildi = 4,

    [Display(Name = "Tamamlandı")]
    Tamamlandi = 5
}