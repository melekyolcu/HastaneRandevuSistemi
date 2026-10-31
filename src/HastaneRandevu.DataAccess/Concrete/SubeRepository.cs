using HastaneRandevu.DataAccess.Abstract;
using HastaneRandevu.DataAccess.Context;
using HastaneRandevu.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.DataAccess.Concrete;

public class SubeRepository : GenericRepository<Sube>, ISubeRepository
{
    public SubeRepository(HastaneRandevuContext context) : base(context) { }

    // Şubeyi kurumu, çalışma saatleri, bölümleri ve bölümlerin hizmetleriyle birlikte getirir
    public async Task<Sube?> DetayliGetirAsync(int id) =>
        await _tablo
            .Include(s => s.Kurum)
            .Include(s => s.CalismaSaatleri)
            .Include(s => s.Bolumler.Where(b => b.AktifMi))
                .ThenInclude(b => b.Hizmetler.Where(h => h.AktifMi))
            .AsSplitQuery()
            .FirstOrDefaultAsync(s => s.Id == id && s.AktifMi);
}