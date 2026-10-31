using HastaneRandevu.Business.Results;
using HastaneRandevu.Entities.Concrete;
using HastaneRandevu.Entities.Enums;

namespace HastaneRandevu.Business.Abstract;

public interface IKurumService
{
    // Kurum
    Task<List<Kurum>> KurumlariListeleAsync();
    Task<Kurum?> KurumGetirAsync(int id);
    Task<IslemSonucu> KurumKaydetAsync(int id, string ad, KurumTuru turu, string? telefon, string? eposta);
    Task<IslemSonucu> KurumSilAsync(int id);

    // Şube
    Task<Sube?> SubeDetayliGetirAsync(int id);
    Task<IslemSonucu> SubeKaydetAsync(int id, int kurumId, string ad, string? sehir, string? adres, string? telefon);
    Task<IslemSonucu> SubeSilAsync(int id);
    Task<IslemSonucu> CalismaSaatiKaydetAsync(int subeId, DayOfWeek gun, bool kapaliMi, TimeOnly acilis, TimeOnly kapanis);

    // Bölüm
    Task<Bolum?> BolumGetirAsync(int id);
    Task<IslemSonucu> BolumKaydetAsync(int id, int subeId, string ad, string? aciklama);
    Task<IslemSonucu> BolumSilAsync(int id);

    // Hizmet
    Task<Hizmet?> HizmetGetirAsync(int id);
    Task<IslemSonucu> HizmetKaydetAsync(int id, int bolumId, string ad, string? aciklama,
        HizmetKategorisi kategori, int sureDakika, decimal ucret);
    Task<IslemSonucu> HizmetSilAsync(int id);
}