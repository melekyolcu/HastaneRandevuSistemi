using HastaneRandevu.Business.Dtos;
using HastaneRandevu.Business.Results;
using HastaneRandevu.Entities.Concrete;

namespace HastaneRandevu.Business.Abstract;

public interface ICalisanService
{
    // Doktor
    Task<List<Doktor>> DoktorlariListeleAsync();
    Task<Doktor?> DoktorGetirAsync(int id);
    Task<IslemSonucu> DoktorKaydetAsync(DoktorBilgisi bilgi);
    Task<IslemSonucu> DoktorSilAsync(int id);
    Task<List<Hizmet>> DoktorunVerebilecegiHizmetlerAsync(int doktorId);
    Task<IslemSonucu> DoktorHizmetleriniGuncelleAsync(int doktorId, IEnumerable<int> hizmetIdleri);

    // Uygunluk
    Task<IslemSonucu> UygunlukEkleAsync(int doktorId, DayOfWeek gun, TimeOnly baslangic, TimeOnly bitis);
    Task<IslemSonucu> UygunlukSilAsync(int uygunlukId, int doktorId);

    // Sağlık personeli
    Task<List<SaglikPersoneli>> PersonelListeleAsync();
    Task<SaglikPersoneli?> PersonelGetirAsync(int id);
    Task<IslemSonucu> PersonelKaydetAsync(PersonelBilgisi bilgi);
    Task<IslemSonucu> PersonelSilAsync(int id);

    // Formlardaki bölüm listesi için
    Task<List<Bolum>> BolumleriListeleAsync();
}