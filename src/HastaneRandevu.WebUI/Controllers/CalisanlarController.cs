using HastaneRandevu.Business.Abstract;
using HastaneRandevu.Business.Dtos;
using HastaneRandevu.Business.Results;
using HastaneRandevu.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HastaneRandevu.WebUI.Controllers;

[Authorize(Roles = "Yonetici")]
public class CalisanlarController : Controller
{
    private readonly ICalisanService _calisanService;

    public CalisanlarController(ICalisanService calisanService)
    {
        _calisanService = calisanService;
    }

    // ---------------- Doktorlar ----------------

    public async Task<IActionResult> Index()
    {
        var doktorlar = await _calisanService.DoktorlariListeleAsync();
        return View(doktorlar);
    }

    [HttpGet]
    public async Task<IActionResult> DoktorForm(int id = 0)
    {
        var model = new DoktorFormViewModel();

        if (id != 0)
        {
            var doktor = await _calisanService.DoktorGetirAsync(id);
            if (doktor is null)
                return NotFound();

            model = new DoktorFormViewModel
            {
                Id = doktor.Id,
                Ad = doktor.Ad,
                Soyad = doktor.Soyad,
                Eposta = doktor.Eposta,
                Telefon = doktor.Telefon,
                Unvan = doktor.Unvan,
                UzmanlikAlani = doktor.UzmanlikAlani,
                SicilNo = doktor.SicilNo,
                IseBaslamaTarihi = doktor.IseBaslamaTarihi,
                BolumId = doktor.BolumId
            };
        }

        await BolumListesiniDoldurAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoktorForm(DoktorFormViewModel model)
    {
        if (model.Id == 0 && string.IsNullOrWhiteSpace(model.Sifre))
            ModelState.AddModelError(nameof(model.Sifre), "Yeni doktor için şifre zorunludur.");

        if (!ModelState.IsValid)
        {
            await BolumListesiniDoldurAsync(model);
            return View(model);
        }

        var bilgi = new DoktorBilgisi(
            model.Id, model.Ad, model.Soyad, model.Eposta, model.Telefon,
            model.Unvan, model.UzmanlikAlani, model.SicilNo, model.IseBaslamaTarihi,
            model.BolumId, model.Sifre);

        var sonuc = await _calisanService.DoktorKaydetAsync(bilgi);
        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            await BolumListesiniDoldurAsync(model);
            return View(model);
        }

        TempData["Basari"] = sonuc.Mesaj;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DoktorDetay(int id)
    {
        var doktor = await _calisanService.DoktorGetirAsync(id);
        if (doktor is null)
            return NotFound();

        var model = new DoktorDetayViewModel
        {
            Doktor = doktor,
            VerilebilecekHizmetler = await _calisanService.DoktorunVerebilecegiHizmetlerAsync(id)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HizmetleriKaydet(int doktorId, List<int> hizmetIdleri)
    {
        BildirimYaz(await _calisanService.DoktorHizmetleriniGuncelleAsync(doktorId, hizmetIdleri));
        return RedirectToAction(nameof(DoktorDetay), new { id = doktorId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UygunlukEkle(int doktorId, DayOfWeek gun, TimeOnly baslangic, TimeOnly bitis)
    {
        BildirimYaz(await _calisanService.UygunlukEkleAsync(doktorId, gun, baslangic, bitis));
        return RedirectToAction(nameof(DoktorDetay), new { id = doktorId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UygunlukSil(int id, int doktorId)
    {
        BildirimYaz(await _calisanService.UygunlukSilAsync(id, doktorId));
        return RedirectToAction(nameof(DoktorDetay), new { id = doktorId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoktorSil(int id)
    {
        BildirimYaz(await _calisanService.DoktorSilAsync(id));
        return RedirectToAction(nameof(Index));
    }

    // ---------------- Sağlık personeli ----------------

    public async Task<IActionResult> Personel()
    {
        var personeller = await _calisanService.PersonelListeleAsync();
        return View(personeller);
    }

    [HttpGet]
    public async Task<IActionResult> PersonelForm(int id = 0)
    {
        if (id == 0)
            return View(new PersonelFormViewModel());

        var personel = await _calisanService.PersonelGetirAsync(id);
        if (personel is null)
            return NotFound();

        return View(new PersonelFormViewModel
        {
            Id = personel.Id,
            Ad = personel.Ad,
            Soyad = personel.Soyad,
            Eposta = personel.Eposta,
            Telefon = personel.Telefon,
            Gorev = personel.Gorev,
            SicilNo = personel.SicilNo,
            IseBaslamaTarihi = personel.IseBaslamaTarihi
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PersonelForm(PersonelFormViewModel model)
    {
        if (model.Id == 0 && string.IsNullOrWhiteSpace(model.Sifre))
            ModelState.AddModelError(nameof(model.Sifre), "Yeni personel için şifre zorunludur.");

        if (!ModelState.IsValid)
            return View(model);

        var bilgi = new PersonelBilgisi(
            model.Id, model.Ad, model.Soyad, model.Eposta, model.Telefon,
            model.Gorev, model.SicilNo, model.IseBaslamaTarihi, model.Sifre);

        var sonuc = await _calisanService.PersonelKaydetAsync(bilgi);
        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(model);
        }

        TempData["Basari"] = sonuc.Mesaj;
        return RedirectToAction(nameof(Personel));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PersonelSil(int id)
    {
        BildirimYaz(await _calisanService.PersonelSilAsync(id));
        return RedirectToAction(nameof(Personel));
    }

    // ---------------- Yardımcı metotlar ----------------

    // Bölümleri "Kurum - Şube" başlıkları altında gruplayarak açılır listeye koyar
    private async Task BolumListesiniDoldurAsync(DoktorFormViewModel model)
    {
        var bolumler = await _calisanService.BolumleriListeleAsync();
        var gruplar = new Dictionary<int, SelectListGroup>();

        model.Bolumler = bolumler.Select(b =>
        {
            if (!gruplar.TryGetValue(b.SubeId, out var grup))
            {
                grup = new SelectListGroup { Name = $"{b.Sube.Kurum.Ad} - {b.Sube.Ad}" };
                gruplar[b.SubeId] = grup;
            }

            return new SelectListItem(b.Ad, b.Id.ToString()) { Group = grup };
        }).ToList();
    }

    private void BildirimYaz(IslemSonucu sonuc)
    {
        TempData[sonuc.Basarili ? "Basari" : "Hata"] = sonuc.Mesaj;
    }
}