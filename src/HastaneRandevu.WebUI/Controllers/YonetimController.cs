using HastaneRandevu.Business.Abstract;
using HastaneRandevu.Business.Results;
using HastaneRandevu.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaneRandevu.WebUI.Controllers;

// Bu controller'daki bütün sayfalara sadece Yönetici rolü girebilir
[Authorize(Roles = "Yonetici")]
public class YonetimController : Controller
{
    private readonly IKurumService _kurumService;

    public YonetimController(IKurumService kurumService)
    {
        _kurumService = kurumService;
    }

    // ---------------- Kurum ----------------

    public async Task<IActionResult> Index()
    {
        var kurumlar = await _kurumService.KurumlariListeleAsync();
        return View(kurumlar);
    }

    [HttpGet]
    public async Task<IActionResult> KurumForm(int id = 0)
    {
        if (id == 0)
            return View(new KurumFormViewModel());

        var kurum = await _kurumService.KurumGetirAsync(id);
        if (kurum is null)
            return NotFound();

        return View(new KurumFormViewModel
        {
            Id = kurum.Id,
            Ad = kurum.Ad,
            Turu = kurum.Turu,
            Telefon = kurum.Telefon,
            Eposta = kurum.Eposta
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KurumForm(KurumFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var sonuc = await _kurumService.KurumKaydetAsync(model.Id, model.Ad, model.Turu, model.Telefon, model.Eposta);
        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(model);
        }

        TempData["Basari"] = sonuc.Mesaj;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KurumSil(int id)
    {
        BildirimYaz(await _kurumService.KurumSilAsync(id));
        return RedirectToAction(nameof(Index));
    }

    // ---------------- Şube ----------------

    public async Task<IActionResult> Sube(int id)
    {
        var sube = await _kurumService.SubeDetayliGetirAsync(id);
        if (sube is null)
            return NotFound();

        return View(sube);
    }

    [HttpGet]
    public async Task<IActionResult> SubeForm(int kurumId, int id = 0)
    {
        if (id == 0)
        {
            var kurum = await _kurumService.KurumGetirAsync(kurumId);
            if (kurum is null)
                return NotFound();

            return View(new SubeFormViewModel { KurumId = kurum.Id, KurumAdi = kurum.Ad });
        }

        var sube = await _kurumService.SubeDetayliGetirAsync(id);
        if (sube is null)
            return NotFound();

        return View(new SubeFormViewModel
        {
            Id = sube.Id,
            KurumId = sube.KurumId,
            KurumAdi = sube.Kurum.Ad,
            Ad = sube.Ad,
            Sehir = sube.Sehir,
            Adres = sube.Adres,
            Telefon = sube.Telefon
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubeForm(SubeFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var sonuc = await _kurumService.SubeKaydetAsync(model.Id, model.KurumId, model.Ad, model.Sehir, model.Adres, model.Telefon);
        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(model);
        }

        TempData["Basari"] = sonuc.Mesaj;
        return model.Id == 0
            ? RedirectToAction(nameof(Index))
            : RedirectToAction(nameof(Sube), new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubeSil(int id)
    {
        var sonuc = await _kurumService.SubeSilAsync(id);
        BildirimYaz(sonuc);

        return sonuc.Basarili
            ? RedirectToAction(nameof(Index))
            : RedirectToAction(nameof(Sube), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CalismaSaatiKaydet(int subeId, DayOfWeek gun, bool kapaliMi, TimeOnly acilis, TimeOnly kapanis)
    {
        BildirimYaz(await _kurumService.CalismaSaatiKaydetAsync(subeId, gun, kapaliMi, acilis, kapanis));
        return RedirectToAction(nameof(Sube), new { id = subeId });
    }

    // ---------------- Bölüm ----------------

    [HttpGet]
    public async Task<IActionResult> BolumForm(int subeId, int id = 0)
    {
        if (id == 0)
            return View(new BolumFormViewModel { SubeId = subeId });

        var bolum = await _kurumService.BolumGetirAsync(id);
        if (bolum is null)
            return NotFound();

        return View(new BolumFormViewModel
        {
            Id = bolum.Id,
            SubeId = bolum.SubeId,
            Ad = bolum.Ad,
            Aciklama = bolum.Aciklama
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BolumForm(BolumFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var sonuc = await _kurumService.BolumKaydetAsync(model.Id, model.SubeId, model.Ad, model.Aciklama);
        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(model);
        }

        TempData["Basari"] = sonuc.Mesaj;
        return RedirectToAction(nameof(Sube), new { id = model.SubeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BolumSil(int id, int subeId)
    {
        BildirimYaz(await _kurumService.BolumSilAsync(id));
        return RedirectToAction(nameof(Sube), new { id = subeId });
    }

    // ---------------- Hizmet ----------------

    [HttpGet]
    public async Task<IActionResult> HizmetForm(int bolumId, int id = 0)
    {
        var model = new HizmetFormViewModel { BolumId = bolumId };

        if (id != 0)
        {
            var hizmet = await _kurumService.HizmetGetirAsync(id);
            if (hizmet is null)
                return NotFound();

            model.Id = hizmet.Id;
            model.BolumId = hizmet.BolumId;
            model.Ad = hizmet.Ad;
            model.Aciklama = hizmet.Aciklama;
            model.Kategori = hizmet.Kategori;
            model.SureDakika = hizmet.SureDakika;
            model.Ucret = (int)Math.Round(hizmet.Ucret);
        }

        var bolum = await _kurumService.BolumGetirAsync(model.BolumId);
        if (bolum is null)
            return NotFound();

        model.SubeId = bolum.SubeId;
        model.BolumAdi = bolum.Ad;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HizmetForm(HizmetFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var sonuc = await _kurumService.HizmetKaydetAsync(
            model.Id, model.BolumId, model.Ad, model.Aciklama,
            model.Kategori, model.SureDakika, model.Ucret);

        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(model);
        }

        TempData["Basari"] = sonuc.Mesaj;
        return RedirectToAction(nameof(Sube), new { id = model.SubeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HizmetSil(int id, int subeId)
    {
        BildirimYaz(await _kurumService.HizmetSilAsync(id));
        return RedirectToAction(nameof(Sube), new { id = subeId });
    }

    // İşlem sonucunu bir sonraki sayfada yeşil veya kırmızı bildirim olarak göstermek için
    private void BildirimYaz(IslemSonucu sonuc)
    {
        TempData[sonuc.Basarili ? "Basari" : "Hata"] = sonuc.Mesaj;
    }
}