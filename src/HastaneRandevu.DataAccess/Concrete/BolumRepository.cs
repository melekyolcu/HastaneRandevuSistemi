using HastaneRandevu.DataAccess.Abstract;
using HastaneRandevu.DataAccess.Context;
using HastaneRandevu.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.DataAccess.Concrete;

public class BolumRepository : GenericRepository<Bolum>, IBolumRepository
{
    public BolumRepository(HastaneRandevuContext context) : base(context) { }

    // Doktor formundaki bölüm listesi için: "Kurum - Şube" başlıkları altında bölümler
    public async Task<List<Bolum>> SubeVeKurumlaListeleAsync() =>
        await _tablo
            .Where(b => b.AktifMi && b.Sube.AktifMi)
            .Include(b => b.Sube).ThenInclude(s => s.Kurum)
            .OrderBy(b => b.Sube.Kurum.Ad).ThenBy(b => b.Sube.Ad).ThenBy(b => b.Ad)
            .ToListAsync();
}