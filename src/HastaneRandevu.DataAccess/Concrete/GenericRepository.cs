using System.Linq.Expressions;
using HastaneRandevu.DataAccess.Abstract;
using HastaneRandevu.DataAccess.Context;
using HastaneRandevu.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.DataAccess.Concrete;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    // protected: türeyen repository'ler (DoktorRepository vb.) de kullanabilsin
    protected readonly HastaneRandevuContext _context;
    protected readonly DbSet<T> _tablo;

    public GenericRepository(HastaneRandevuContext context)
    {
        _context = context;
        _tablo = context.Set<T>();
    }

    public virtual async Task<T?> GetirAsync(int id) =>
        await _tablo.FirstOrDefaultAsync(x => x.Id == id && x.AktifMi);

    public virtual async Task<List<T>> ListeleAsync(Expression<Func<T, bool>>? filtre = null)
    {
        IQueryable<T> sorgu = _tablo.Where(x => x.AktifMi);
        if (filtre is not null)
            sorgu = sorgu.Where(filtre);
        return await sorgu.ToListAsync();
    }

    public async Task<bool> VarMiAsync(Expression<Func<T, bool>> filtre) =>
        await _tablo.Where(x => x.AktifMi).AnyAsync(filtre);

    public async Task EkleAsync(T varlik)
    {
        await _tablo.AddAsync(varlik);
        await _context.SaveChangesAsync();
    }

    public async Task GuncelleAsync(T varlik)
    {
        varlik.GuncellemeTarihi = DateTime.UtcNow;
        _tablo.Update(varlik);
        await _context.SaveChangesAsync();
    }

    
    /// Kayıt fiziksel olarak silinmez, pasif yapılır (soft delete).
    /// Böylece geçmiş randevular ve kayıtlar kaybolmaz.
    
    public async Task SilAsync(T varlik)
    {
        varlik.AktifMi = false;
        await GuncelleAsync(varlik);
    }
}