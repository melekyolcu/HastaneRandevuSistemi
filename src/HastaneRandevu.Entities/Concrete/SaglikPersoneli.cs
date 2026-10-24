using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Entities.Concrete;

public class SaglikPersoneli : Calisan
{
    public string Gorev { get; set; } = string.Empty;   

    public override KullaniciRolu Rol => KullaniciRolu.SaglikPersoneli;

    public override string BilgiGetir() => $"{AdSoyad} - {Gorev}";
}