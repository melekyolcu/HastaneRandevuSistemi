using HastaneRandevu.Business.Results;
using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Concrete;

namespace HastaneRandevu.Business.Abstract;

public interface IKullaniciService
{
    Task<IslemSonucu<Kullanici>> GirisYapAsync(string eposta, string sifre);

    Task<IslemSonucu<Hasta>> HastaKayitAsync(
        string ad, string soyad, string eposta, string telefon,
        string tcKimlikNo, DateTime dogumTarihi, string sifre);

    Task<Kullanici?> GetirAsync(int id);
}