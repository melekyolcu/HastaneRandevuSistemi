using HastaneRandevu.Entities.Abstract;

namespace HastaneRandevu.Entities.Concrete;

/// Doktorun randevu kabul ettiği haftalık zaman dilimi

public class UygunlukZamani : BaseEntity
{
    public DayOfWeek Gun { get; set; }
    public TimeOnly BaslangicSaati { get; private set; }
    public TimeOnly BitisSaati { get; private set; }

    public int DoktorId { get; set; }
    public Doktor Doktor { get; set; } = null!;

    public void SaatleriAyarla(TimeOnly baslangic, TimeOnly bitis)
    {
        if (bitis <= baslangic)
            throw new ArgumentException("Bitiş saati, başlangıç saatinden sonra olmalıdır.");

        BaslangicSaati = baslangic;
        BitisSaati = bitis;
    }

   
    /// İstenen randevu aralığı bu dilimin içinde mi
   
    public bool Kapsar(DateTime randevuBaslangic, DateTime randevuBitis)
    {
        if (randevuBaslangic.Date != randevuBitis.Date) return false;
        if (randevuBaslangic.DayOfWeek != Gun) return false;

        var baslangic = TimeOnly.FromDateTime(randevuBaslangic);
        var bitis = TimeOnly.FromDateTime(randevuBitis);

        return baslangic >= BaslangicSaati && bitis <= BitisSaati;
    }
}