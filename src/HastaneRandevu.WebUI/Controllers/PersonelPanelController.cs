using HastaneRandevu.Business.Abstract;
using HastaneRandevu.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaneRandevu.WebUI.Controllers;

// Sağlık personeli günün bütün randevularını görebilir, değişiklik yapamaz
[Authorize(Roles = "SaglikPersoneli")]
public class PersonelPanelController : Controller
{
    private readonly IRandevuService _randevuService;

    public PersonelPanelController(IRandevuService randevuService)
    {
        _randevuService = randevuService;
    }

    public async Task<IActionResult> Index(DateTime? tarih)
    {
        var gun = (tarih ?? DateTime.Today).Date;

        var model = new GunlukRandevularViewModel
        {
            Tarih = gun,
            Randevular = await _randevuService.GununTumRandevulariAsync(gun)
        };

        return View(model);
    }
}