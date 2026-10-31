using System.ComponentModel.DataAnnotations;

namespace HastaneRandevu.Entities.Enums;

public enum KurumTuru
{
    Hastane = 1,
    Klinik = 2,

    [Display(Name = "Tıp Merkezi")]
    TipMerkezi = 3
}