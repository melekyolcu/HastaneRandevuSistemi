using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Entities.Concrete;

public class Yonetici : Kullanici
{
    public override KullaniciRolu Rol => KullaniciRolu.Yonetici;
}