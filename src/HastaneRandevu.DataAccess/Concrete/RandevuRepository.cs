using HastaneRandevu.DataAccess.Abstract;
using HastaneRandevu.DataAccess.Context;
using HastaneRandevu.Entities.Concrete;
using HastaneRandevu.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace HastaneRandevu.DataAccess.Concrete;

public class RandevuRepository : GenericRepository<Randevu>, IRandevuRepository
{
    public RandevuRepository(HastaneRandevuContext context) : base(context) { }

    public async Task<List<Randevu>> DoktorunGunlukRandevulariAsync(int doktorId, DateTime gun)
    {
        var gunBaslangici = DateTime.SpecifyKind(gun.Date, DateTimeKind.Unspecified);
        var gunSonu = gunBaslangici.AddDays(1);

        return await _tablo
            .Where(r => r.AktifMi
                        && r.DoktorId == doktorId
                        && r.BaslangicZamani >= gunBaslangici
                        && r.BaslangicZamani < gunSonu)
            .Include(r => r.Hasta)
            .Include(r => r.Hizmet)
            .OrderBy(r => r.BaslangicZamani)
            .ToListAsync();
    }

    
    /// Doktorun o günkü randevularını çeker ve her birine
    /// Randevu sınıfındaki CakisiyorMu() metoduyla sorar.
    
    public async Task<bool> CakismaVarMiAsync(int doktorId, DateTime baslangic, DateTime bitis)
    {
        var gununRandevulari = await DoktorunGunlukRandevulariAsync(doktorId, baslangic);
        return gununRandevulari.Any(r => r.CakisiyorMu(baslangic, bitis));
    }
    
    // Hasta aynı saatte başka bir doktordan randevu almış olabilir
    public async Task<bool> HastaninCakismasiVarMiAsync(int hastaId, DateTime baslangic, DateTime bitis)
    {
        var gunBaslangici = DateTime.SpecifyKind(baslangic.Date, DateTimeKind.Unspecified);
        var gunSonu = gunBaslangici.AddDays(1);

        var gununRandevulari = await _tablo
            .Where(r => r.AktifMi
                        && r.HastaId == hastaId
                        && r.BaslangicZamani >= gunBaslangici
                        && r.BaslangicZamani < gunSonu)
            .ToListAsync();

        return gununRandevulari.Any(r => r.CakisiyorMu(baslangic, bitis));
    }

    public async Task<List<Randevu>> HastaninRandevulariAsync(int hastaId) =>
        await _tablo
            .Where(r => r.AktifMi && r.HastaId == hastaId)
            .Include(r => r.Doktor)
            .Include(r => r.Hizmet)
            .OrderByDescending(r => r.BaslangicZamani)
            .ToListAsync();

    public async Task<List<Randevu>> OnayBekleyenlerAsync(int doktorId) =>
        await _tablo
            .Where(r => r.AktifMi && r.DoktorId == doktorId && r.Durum == RandevuDurumu.Beklemede)
            .Include(r => r.Hasta)
            .Include(r => r.Hizmet)
            .OrderBy(r => r.BaslangicZamani)
            .ToListAsync();
}