using HastaneRandevu.Business.Abstract;
using HastaneRandevu.Business.Results;
using HastaneRandevu.WebUI.Helpers;
using HastaneRandevu.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaneRandevu.WebUI.Controllers;

[Authorize(Roles = "Doktor")]
public class DoktorPanelController : Controller
{
    private readonly IRandevuService _randevuService;
    private readonly ICalisanService _calisanService;

    public DoktorPanelController(IRandevuService randevuService, ICalisanService calisanService)
    {
        _randevuService = randevuService;
        _calisanService = calisanService;
    }

    // Giriş yapmış doktorun Id'si; doktor sadece kendi kayıtları üzerinde işlem yapabilir
    private int DoktorId => User.KullaniciId();

    // ---------------- Randevular ----------------

    public async Task<IActionResult> Index(DateTime? tarih)
    {
        var gun = (tarih ?? DateTime.Today).Date;

        var onayBekleyenler = (await _randevuService.OnayBekleyenlerAsync(DoktorId))
            .Where(r => r.BaslangicZamani >= DateTime.Today)
            .ToList();

        var model = new DoktorPanelViewModel
        {
            Tarih = gun,
            GununRandevulari = await _randevuService.DoktorunGunlukRandevulariAsync(DoktorId, gun),
            OnayBekleyenler = onayBekleyenler
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Onayla(int id, DateTime? tarih)
    {
        BildirimYaz(await _randevuService.OnaylaAsync(id, DoktorId));
        return IndexeDon(tarih);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reddet(int id, string neden, DateTime? tarih)
    {
        BildirimYaz(await _randevuService.ReddetAsync(id, DoktorId, neden));
        return IndexeDon(tarih);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Tamamla(int id, DateTime? tarih)
    {
        BildirimYaz(await _randevuService.TamamlaAsync(id, DoktorId));
        return IndexeDon(tarih);
    }

    // ---------------- Çalışma saatleri ----------------

    public async Task<IActionResult> Saatlerim()
    {
        var doktor = await _calisanService.DoktorGetirAsync(DoktorId);
        if (doktor is null)
            return NotFound();

        return View(doktor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UygunlukEkle(DayOfWeek gun, TimeOnly baslangic, TimeOnly bitis)
    {
        BildirimYaz(await _calisanService.UygunlukEkleAsync(DoktorId, gun, baslangic, bitis));
        return RedirectToAction(nameof(Saatlerim));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UygunlukSil(int id)
    {
        BildirimYaz(await _calisanService.UygunlukSilAsync(id, DoktorId));
        return RedirectToAction(nameof(Saatlerim));
    }

    // ---------------- Yardımcı metotlar ----------------

    private IActionResult IndexeDon(DateTime? tarih) =>
        RedirectToAction(nameof(Index), new { tarih = tarih?.ToString("yyyy-MM-dd") });

    private void BildirimYaz(IslemSonucu sonuc)
    {
        TempData[sonuc.Basarili ? "Basari" : "Hata"] = sonuc.Mesaj;
    }
}