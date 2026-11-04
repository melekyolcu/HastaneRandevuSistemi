using HastaneRandevu.Business.Abstract;
using HastaneRandevu.Business.Results;
using HastaneRandevu.WebUI.Helpers;
using HastaneRandevu.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaneRandevu.WebUI.Controllers;

[Authorize(Roles = "Hasta")]
public class RandevuController : Controller
{
    private readonly IRandevuService _randevuService;
    private readonly IKurumService _kurumService;

    public RandevuController(IRandevuService randevuService, IKurumService kurumService)
    {
        _randevuService = randevuService;
        _kurumService = kurumService;
    }

    private int HastaId => User.KullaniciId();

    public async Task<IActionResult> Index()
    {
        var randevular = await _randevuService.HastaninRandevulariAsync(HastaId);
        return View(randevular);
    }

    // Adım adım randevu alma: hizmet → doktor → gün → saat
    public async Task<IActionResult> Al(int? hizmetId, int? doktorId, DateTime? tarih)
    {
        var gun = (tarih ?? DateTime.Today).Date;
        if (gun < DateTime.Today)
            gun = DateTime.Today;

        var model = new RandevuAlViewModel
        {
            Bolumler = await _randevuService.RandevuyaAcikBolumlerAsync(),
            HizmetId = hizmetId,
            DoktorId = doktorId,
            Tarih = gun
        };

        if (hizmetId is int secilenHizmet)
        {
            model.Hizmet = await _kurumService.HizmetGetirAsync(secilenHizmet);
            model.Doktorlar = await _randevuService.HizmetiVerenDoktorlarAsync(secilenHizmet);
        }

        // Hizmet değiştirildiyse ve önceki doktor bu hizmeti vermiyorsa doktor seçimi sıfırlanır
        if (model.DoktorId is int secilenDoktor && model.Doktorlar.All(d => d.Id != secilenDoktor))
            model.DoktorId = null;

        if (model.HizmetId is int h && model.DoktorId is int d)
            model.BosSaatler = await _randevuService.UygunSaatleriGetirAsync(d, h, model.Tarih);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Olustur(int hizmetId, int doktorId, DateTime baslangic, string? hastaNotu)
    {
        var sonuc = await _randevuService.OlusturAsync(HastaId, doktorId, hizmetId, baslangic, hastaNotu);

        if (!sonuc.Basarili)
        {
            TempData["Hata"] = sonuc.Mesaj;
            return RedirectToAction(nameof(Al), new { hizmetId, doktorId, tarih = baslangic.ToString("yyyy-MM-dd") });
        }

        TempData["Basari"] = sonuc.Mesaj;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IptalEt(int id)
    {
        var sonuc = await _randevuService.IptalEtAsync(id, HastaId);
        TempData[sonuc.Basarili ? "Basari" : "Hata"] = sonuc.Mesaj;
        return RedirectToAction(nameof(Index));
    }
}