using HastaneRandevu.Entities.Concrete;

namespace HastaneRandevu.DataAccess.Abstract;

public interface IBolumRepository : IGenericRepository<Bolum>
{
    Task<List<Bolum>> SubeVeKurumlaListeleAsync();
    Task<List<Bolum>> HizmetleriyleListeleAsync();
    
}