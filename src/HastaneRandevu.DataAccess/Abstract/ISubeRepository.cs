using HastaneRandevu.Entities.Concrete;

namespace HastaneRandevu.DataAccess.Abstract;

public interface ISubeRepository : IGenericRepository<Sube>
{
    Task<Sube?> DetayliGetirAsync(int id);
}