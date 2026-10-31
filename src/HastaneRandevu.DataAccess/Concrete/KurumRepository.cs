using HastaneRandevu.DataAccess.Abstract;
using HastaneRandevu.DataAccess.Context;
using HastaneRandevu.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.DataAccess.Concrete;

public class KurumRepository : GenericRepository<Kurum>, IKurumRepository
{
    public KurumRepository(HastaneRandevuContext context) : base(context) { }

    // Silinmiş (pasif) şubeler listeye gelmesin
    public async Task<List<Kurum>> SubeleriyleListeleAsync() =>
        await _tablo
            .Where(k => k.AktifMi)
            .Include(k => k.Subeler.Where(s => s.AktifMi))
            .OrderBy(k => k.Ad)
            .ToListAsync();

    public async Task<Kurum?> SubeleriyleGetirAsync(int id) =>
        await _tablo
            .Include(k => k.Subeler.Where(s => s.AktifMi))
            .FirstOrDefaultAsync(k => k.Id == id && k.AktifMi);
}