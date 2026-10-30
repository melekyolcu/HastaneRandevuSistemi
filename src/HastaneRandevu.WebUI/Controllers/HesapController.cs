using System.Security.Claims;
using HastaneRandevu.Business.Abstract;
using HastaneRandevu.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaneRandevu.WebUI.Controllers;

public class HesapController : Controller
{
    private readonly IKullaniciService _kullaniciService;

    public HesapController(IKullaniciService kullaniciService)
    {
        _kullaniciService = kullaniciService;
    }

    [HttpGet]
    public IActionResult Giris(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Giris(GirisViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var sonuc = await _kullaniciService.GirisYapAsync(model.Eposta, model.Sifre);
        if (!sonuc.Basarili || sonuc.Veri is null)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(model);
        }

        var kullanici = sonuc.Veri;

        // Çerezde tutulacak bilgiler: kim olduğu ve rolü
        var bilgiler = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
            new(ClaimTypes.Name, kullanici.AdSoyad),
            new(ClaimTypes.Email, kullanici.Eposta),
            new(ClaimTypes.Role, kullanici.Rol.ToString())
        };

        var kimlik = new ClaimsIdentity(bilgiler, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(kimlik),
            new AuthenticationProperties { IsPersistent = model.BeniHatirla });

        TempData["Basari"] = sonuc.Mesaj;

        // Kullanıcı kilitli bir sayfaya girmeye çalışırken girişe yönlendirildiyse oraya geri gönder
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Kayit()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Kayit(KayitViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var sonuc = await _kullaniciService.HastaKayitAsync(
            model.Ad, model.Soyad, model.Eposta, model.Telefon ?? string.Empty,
            model.TcKimlikNo, model.DogumTarihi!.Value, model.Sifre);

        if (!sonuc.Basarili)
        {
            ModelState.AddModelError(string.Empty, sonuc.Mesaj);
            return View(model);
        }

        TempData["Basari"] = sonuc.Mesaj;
        return RedirectToAction(nameof(Giris));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cikis()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Giris));
    }

    // Rolü yetmeyen bir sayfaya girmeye çalışınca buraya gelir
    public IActionResult ErisimEngellendi() => View();
}