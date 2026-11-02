namespace HastaneRandevu.Business.Dtos;

// Formdan gelen doktor bilgilerini service'e tek parça halinde taşır
public record DoktorBilgisi(
    int Id,
    string Ad,
    string Soyad,
    string Eposta,
    string? Telefon,
    string Unvan,
    string UzmanlikAlani,
    string? SicilNo,
    DateTime? IseBaslamaTarihi,
    int BolumId,
    string? Sifre);

public record PersonelBilgisi(
    int Id,
    string Ad,
    string Soyad,
    string Eposta,
    string? Telefon,
    string Gorev,
    string? SicilNo,
    DateTime? IseBaslamaTarihi,
    string? Sifre);