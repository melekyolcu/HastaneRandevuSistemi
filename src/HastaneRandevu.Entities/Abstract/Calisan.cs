namespace HastaneRandevu.Entities.Abstract;

/// Doktor ve sağlık personelinin ortak özellikleri.
/// Çok seviyeli kalıtım: BaseEntity → Kullanici → Calisan → Doktor

public abstract class Calisan : Kullanici
{
    public string SicilNo { get; set; } = string.Empty;
    public DateTime IseBaslamaTarihi { get; set; }
}