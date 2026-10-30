using HastaneRandevu.Business.Abstract;
using HastaneRandevu.Business.Results;
using HastaneRandevu.DataAccess.Abstract;
using HastaneRandevu.Entities.Abstract;
using HastaneRandevu.Entities.Concrete;

namespace HastaneRandevu.Business.Concrete;

public class KullaniciManager : IKullaniciService
{
    private readonly IGenericRepository<Kullanici> _kullaniciRepository;
    private readonly IGenericRepository<Hasta> _hastaRepository;

    public KullaniciManager(
        IGenericRepository<Kullanici> kullaniciRepository,
        IGenericRepository<Hasta> hastaRepository)
    {
        _kullaniciRepository = kullaniciRepository;
        _hastaRepository = hastaRepository;
    }

    public async Task<IslemSonucu<Kullanici>> GirisYapAsync(string eposta, string sifre)
    {
        if (string.IsNullOrWhiteSpace(eposta) || string.IsNullOrWhiteSpace(sifre))
            return IslemSonucu<Kullanici>.Hata("E-posta ve şifre boş bırakılamaz.");

        var normalEposta = eposta.Trim().ToLowerInvariant();
        var kullanici = (await _kullaniciRepository.ListeleAsync(k => k.Eposta == normalEposta))
            .FirstOrDefault();

        //  sistemde hangi e-postaların kayıtlı olduğu anlaşılamaz
        if (kullanici is null || !kullanici.SifreDogruMu(sifre))
            return IslemSonucu<Kullanici>.Hata("E-posta veya şifre hatalı.");

        return IslemSonucu<Kullanici>.Basari(kullanici, $"Hoş geldiniz, {kullanici.AdSoyad}.");
    }

    public async Task<IslemSonucu<Hasta>> HastaKayitAsync(
        string ad, string soyad, string eposta, string telefon,
        string tcKimlikNo, DateTime dogumTarihi, string sifre)
    {
        var normalEposta = (eposta ?? string.Empty).Trim().ToLowerInvariant();
        var tc = (tcKimlikNo ?? string.Empty).Trim();

        if (await _kullaniciRepository.VarMiAsync(k => k.Eposta == normalEposta))
            return IslemSonucu<Hasta>.Hata("Bu e-posta adresiyle kayıtlı bir kullanıcı zaten var.");

        if (await _hastaRepository.VarMiAsync(h => h.TcKimlikNo == tc))
            return IslemSonucu<Hasta>.Hata("Bu TC Kimlik No ile kayıtlı bir hasta zaten var.");

        if (dogumTarihi.Date > DateTime.Today)
            return IslemSonucu<Hasta>.Hata("Doğum tarihi gelecekte olamaz.");

        Hasta hasta;
        try
        {
            // Ad, e-posta, TC gibi alanların kontrolünü Hasta class'ının kendisi yapıyor
            hasta = new Hasta
            {
                Ad = ad,
                Soyad = soyad,
                Eposta = normalEposta,
                Telefon = telefon ?? string.Empty,
                TcKimlikNo = tc,
                DogumTarihi = dogumTarihi.Date
            };
            hasta.SifreBelirle(sifre);
        }
        catch (ArgumentException ex)
        {
            return IslemSonucu<Hasta>.Hata(ex.Message);
        }

        await _hastaRepository.EkleAsync(hasta);
        return IslemSonucu<Hasta>.Basari(hasta, "Kaydınız oluşturuldu, giriş yapabilirsiniz.");
    }

    public Task<Kullanici?> GetirAsync(int id) => _kullaniciRepository.GetirAsync(id);
}