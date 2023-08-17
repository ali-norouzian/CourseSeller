using CourseSeller.Core.DTOs.UserPanel.Wallet;
using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Areas.UserPanel.Controllers;

[Area("UserPanel")]
[Authorize]
[Route("/[area]/[controller]/[action]")]
public class WalletController : Controller
{
    private IUserPanelService _userPanelService;

    public WalletController(IUserPanelService userPanelService)
    {
        _userPanelService = userPanelService;
    }


    [Route("/[area]/[controller]")]
    public async Task<IActionResult> Index()
    {
        ViewData["WalletList"] = await _userPanelService.GetUserWallet(User.Identity.Name);

        return View();
    }

    [HttpPost]
    [Route("/[area]/[controller]")]
    public async Task<IActionResult> Index(ChargeWalletViewModel viewModel)
    {
        bool errorFlag = !ModelState.IsValid;
        if (errorFlag)
            return View(viewModel);

        // Charge wallet
        await _userPanelService.ChargeWallet(User.Identity.Name, viewModel.Amount);

        // ToDo: online payment

        return RedirectToAction(nameof(Index));
    }

}