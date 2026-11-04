using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HastaneRandevu.WebUI.Helpers;

public static class EnumHelper
{
    // Enum değerine [Display(Name = "...")] ile verilen adı döndürür, yoksa kendi adını
    public static string GorunenAd(this Enum deger)
    {
        var alan = deger.GetType().GetField(deger.ToString());
        return alan?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? deger.ToString();
    }
    
    // Randevu durumuna göre Bootstrap rozet rengi
    public static string Renk(this HastaneRandevu.Entities.Enums.RandevuDurumu durum) => durum switch
    {
        HastaneRandevu.Entities.Enums.RandevuDurumu.Beklemede => "warning",
        HastaneRandevu.Entities.Enums.RandevuDurumu.Onaylandi => "success",
        HastaneRandevu.Entities.Enums.RandevuDurumu.Reddedildi => "danger",
        HastaneRandevu.Entities.Enums.RandevuDurumu.IptalEdildi => "secondary",
        HastaneRandevu.Entities.Enums.RandevuDurumu.Tamamlandi => "primary",
        _ => "light"
    };
}