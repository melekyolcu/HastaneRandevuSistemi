using HastaneRandevu.Business.Results;
using HastaneRandevu.Entities.Concrete;

namespace HastaneRandevu.Business.Abstract;

public interface IRandevuService
{
    // Hasta işlemleri
    Task<IslemSonucu<Randevu>> OlusturAsync(int hastaId, int doktorId, int hizmetId, DateTime baslangic, string? hastaNotu = null);
    Task<IslemSonucu> IptalEtAsync(int randevuId, int hastaId);
    Task<List<Randevu>> HastaninRandevulariAsync(int hastaId);
    Task<List<DateTime>> UygunSaatleriGetirAsync(int doktorId, int hizmetId, DateTime gun);

    // Doktor işlemleri
    Task<IslemSonucu> OnaylaAsync(int randevuId, int doktorId);
    Task<IslemSonucu> ReddetAsync(int randevuId, int doktorId, string neden);
    Task<IslemSonucu> TamamlaAsync(int randevuId, int doktorId);
    Task<List<Randevu>> OnayBekleyenlerAsync(int doktorId);
    Task<List<Randevu>> DoktorunGunlukRandevulariAsync(int doktorId, DateTime gun);
}