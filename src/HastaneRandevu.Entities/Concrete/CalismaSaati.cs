using HastaneRandevu.Entities.Abstract;

namespace HastaneRandevu.Entities.Concrete;

/// Bir şubenin haftanın belirli bir günündeki çalışma saatleri.
public class CalismaSaati : BaseEntity
{
    public DayOfWeek Gun { get; set; }
    public bool KapaliMi { get; set; }

    // Kapsülleme: saatler dışarıdan tek tek değiştirilemez (private set),
    // sadece SaatleriAyarla metoduyla, kontrol edilerek atanır.
    public TimeOnly AcilisSaati { get; private set; }
    public TimeOnly KapanisSaati { get; private set; }

    public int SubeId { get; set; }
    public Sube Sube { get; set; } = null!;

    public void SaatleriAyarla(TimeOnly acilis, TimeOnly kapanis)
    {
        if (kapanis <= acilis)
            throw new ArgumentException("Kapanış saati, açılış saatinden sonra olmalıdır.");

        AcilisSaati = acilis;
        KapanisSaati = kapanis;
        KapaliMi = false;
    }

    public bool AcikMi(TimeOnly saat) =>
        !KapaliMi && saat >= AcilisSaati && saat < KapanisSaati;
}