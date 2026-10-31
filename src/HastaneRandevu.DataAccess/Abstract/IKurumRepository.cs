using HastaneRandevu.Entities.Concrete;

namespace HastaneRandevu.DataAccess.Abstract;

public interface IKurumRepository : IGenericRepository<Kurum>
{
    Task<List<Kurum>> SubeleriyleListeleAsync();
    Task<Kurum?> SubeleriyleGetirAsync(int id);
}