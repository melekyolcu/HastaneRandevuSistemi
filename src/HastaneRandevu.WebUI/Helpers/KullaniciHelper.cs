using System.Security.Claims;

namespace HastaneRandevu.WebUI.Helpers;

public static class KullaniciHelper
{
    // Giriş yapmış kullanıcının Id'si (girişte çereze yazdığımız NameIdentifier bilgisi)
    public static int KullaniciId(this ClaimsPrincipal kullanici) =>
        int.TryParse(kullanici.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}