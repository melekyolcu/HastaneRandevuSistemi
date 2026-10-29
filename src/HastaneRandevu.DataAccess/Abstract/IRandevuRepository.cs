using HastaneRandevu.Entities.Concrete;

namespace HastaneRandevu.DataAccess.Abstract;

public interface IRandevuRepository : IGenericRepository<Randevu>
{
    Task<List<Randevu>> DoktorunGunlukRandevulariAsync(int doktorId, DateTime gun);
    Task<bool> CakismaVarMiAsync(int doktorId, DateTime baslangic, DateTime bitis);
    Task<List<Randevu>> HastaninRandevulariAsync(int hastaId);
    Task<List<Randevu>> OnayBekleyenlerAsync(int doktorId);
}