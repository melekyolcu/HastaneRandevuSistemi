using System.ComponentModel.DataAnnotations;

namespace HastaneRandevu.Entities.Enums;

public enum HizmetKategorisi
{
    Muayene = 1,
    Laboratuvar = 2,
    Radyoloji = 3,
    Tedavi = 4,
   
    [Display(Name = "Diğer")]
    Diger = 99
}