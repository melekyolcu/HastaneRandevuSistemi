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
}