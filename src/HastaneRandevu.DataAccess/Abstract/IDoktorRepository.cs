using HastaneRandevu.Entities.Concrete;

namespace HastaneRandevu.DataAccess.Abstract;

public interface IDoktorRepository : IGenericRepository<Doktor>
{
    /// Doktoru bölümü, hizmetleri ve uygunluk saatleriyle birlikte getirir.
    Task<Doktor?> DetayliGetirAsync(int id);

    /// Belirli bir hizmeti verebilen doktorları listeler (hasta randevu alırken).
    Task<List<Doktor>> HizmeteGoreListeleAsync(int hizmetId);
}