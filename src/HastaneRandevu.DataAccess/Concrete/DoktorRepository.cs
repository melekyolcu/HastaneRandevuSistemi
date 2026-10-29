using HastaneRandevu.DataAccess.Abstract;
using HastaneRandevu.DataAccess.Context;
using HastaneRandevu.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.DataAccess.Concrete;

public class DoktorRepository : GenericRepository<Doktor>, IDoktorRepository
{
    public DoktorRepository(HastaneRandevuContext context) : base(context) { }

    public async Task<Doktor?> DetayliGetirAsync(int id) =>
        await _tablo
            .Include(d => d.Bolum).ThenInclude(b => b.Sube)
            .Include(d => d.Hizmetler)
            .Include(d => d.UygunlukZamanlari)
            .FirstOrDefaultAsync(d => d.Id == id && d.AktifMi);

    public async Task<List<Doktor>> HizmeteGoreListeleAsync(int hizmetId) =>
        await _tablo
            .Where(d => d.AktifMi && d.Hizmetler.Any(h => h.Id == hizmetId))
            .Include(d => d.Bolum)
            .OrderBy(d => d.Ad)
            .ToListAsync();
}