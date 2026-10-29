using System.Linq.Expressions;
using HastaneRandevu.Entities.Abstract;

namespace HastaneRandevu.DataAccess.Abstract;


/// Tüm tablolar için ortak veritabanı işlemleri.
///  BaseEntity'den türeyen herhangi bir sınıf olabilir (Hasta, Hizmet, Randevu...).

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetirAsync(int id);
    Task<List<T>> ListeleAsync(Expression<Func<T, bool>>? filtre = null);
    Task<bool> VarMiAsync(Expression<Func<T, bool>> filtre);
    Task EkleAsync(T varlik);
    Task GuncelleAsync(T varlik);
    Task SilAsync(T varlik);
}