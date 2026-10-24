using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Entities.Concrete;

public class Doktor : Calisan
{
    public string Unvan { get; set; } = "Dr.";  
    public string UzmanlikAlani { get; set; } = string.Empty;

    public override KullaniciRolu Rol => KullaniciRolu.Doktor;

    public override string BilgiGetir() => $"{Unvan} {AdSoyad} - {UzmanlikAlani}";
}