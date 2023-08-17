using CourseSeller.Core.DTOs.UserPanel.Wallet;
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Entities.Wallets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Areas.UserPanel.Controllers;

[Area("UserPanel")]
[Authorize]
[Route("/[area]/[controller]/[action]")]
public class WalletController : Controller
{
    private readonly IUserPanelService _userPanelService;
    private readonly IConfiguration _conf;

    public WalletController(IUserPanelService userPanelService, IConfiguration conf)
    {
        _userPanelService = userPanelService;
        _conf = conf;
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
        viewModel.Amount = Math.Abs(viewModel.Amount);
        var walletId = await _userPanelService.AddWallet(User.Identity.Name, viewModel.Amount);

        // Online payment

        #region Online payment

        var callbackUrl = $"{_conf["DomainLink"]}/OnlinePayment/{walletId}";
        var payment = new ZarinpalSandbox.Payment(viewModel.Amount);
        var response =
            await payment.PaymentRequest("شارژ کیف پول", callbackUrl);
        // 100 mean its ok
        if (response.Status == 100)
            return Redirect($"https://sandbox.zarinpal.com/pg/StartPay/{response.Authority}");

        #endregion

        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [Route("/OnlinePayment/{id}")]
    public async Task<IActionResult> OnlinePayment(int id)
    {
        if (
            HttpContext.Request.Query["Authority"] != "")
        {
            var wallet = await _userPanelService.GetWalletById(id);
            var amount = Math.Abs(wallet.Amount);

            var authority = HttpContext.Request.Query["Authority"];
            var payment = new ZarinpalSandbox.Payment(amount);
            var response = await payment.Verification(authority);
            if (response.Status == 100)
            {
                ViewData["RefId"] = response.RefId;
                ViewData["IsSuccess"] = true;

                await _userPanelService.SetWalletIsPaidAndChargeTransaction(wallet, User.Identity.Name, amount);
            }
            else
            {
                ViewData["RefId"] = response.RefId;
            }
        }

        return View();
    }
}