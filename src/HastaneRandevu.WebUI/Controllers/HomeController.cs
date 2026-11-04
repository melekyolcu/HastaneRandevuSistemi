using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HastaneRandevu.WebUI.Models;

namespace HastaneRandevu.WebUI.Controllers;

public class HomeController : Controller
{
    // Giriş yapmış kullanıcıyı rolüne göre kendi paneline yönlendirir
    public IActionResult Index()
    {
        if (User.IsInRole("Yonetici"))
            return RedirectToAction("Index", "Yonetim");
        if (User.IsInRole("Doktor"))
            return RedirectToAction("Index", "DoktorPanel");
        if (User.IsInRole("SaglikPersoneli"))
            return RedirectToAction("Index", "PersonelPanel");
        if (User.IsInRole("Hasta"))
            return RedirectToAction("Index", "Randevu");

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
