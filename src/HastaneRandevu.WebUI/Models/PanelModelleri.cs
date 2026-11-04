using HastaneRandevu.Entities.Concrete;

namespace HastaneRandevu.WebUI.Models;

public class DoktorPanelViewModel
{
    public DateTime Tarih { get; set; }
    public List<Randevu> GununRandevulari { get; set; } = new();
    public List<Randevu> OnayBekleyenler { get; set; } = new();
}

public class RandevuAlViewModel
{
    // 1. adım: hizmet
    public List<Bolum> Bolumler { get; set; } = new();
    public int? HizmetId { get; set; }
    public Hizmet? Hizmet { get; set; }

    // 2. adım: doktor
    public List<Doktor> Doktorlar { get; set; } = new();
    public int? DoktorId { get; set; }

    // 3. adım: gün ve saat
    public DateTime Tarih { get; set; }
    public List<DateTime> BosSaatler { get; set; } = new();
}

public class GunlukRandevularViewModel
{
    public DateTime Tarih { get; set; }
    public List<Randevu> Randevular { get; set; } = new();
}