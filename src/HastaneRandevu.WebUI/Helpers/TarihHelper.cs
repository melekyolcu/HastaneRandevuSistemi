using System.Globalization;

namespace HastaneRandevu.WebUI.Helpers;

public static class TarihHelper
{
    private static readonly CultureInfo Turkce = new("tr-TR");

    // Haftayı Pazartesi'den başlatmak için (C#'ta varsayılan sıra Pazar'dan başlıyor bu yüzden bunu kullandım)
    public static readonly DayOfWeek[] HaftaGunleri =
    {
        DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
        DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
    };

    public static string GunAdi(this DayOfWeek gun) => Turkce.DateTimeFormat.GetDayName(gun);
    
    // Örneiğin "4 Kasım 2026, Çarşamba"
    public static string TarihYaz(this DateTime tarih) => tarih.ToString("d MMMM yyyy, dddd", Turkce);
}