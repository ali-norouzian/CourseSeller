using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Areas.UserPanel.Controllers;


[Area("UserPanel")]
[Authorize]
[Route("[area]/[action]")]
public class HomeController : Controller
{
    private readonly IUserPanelService _userPanelService;
    private readonly IAccountService _accountService;

    public HomeController(IUserPanelService userPanelService, IAccountService accountService)
    {
        _userPanelService = userPanelService;
        _accountService = accountService;
    }

    [Route("/[area]")]
    public async Task<IActionResult> Index()
    {
        var UserPanelInfo = await _userPanelService.GetUserInfo(User.Identity.Name);
        ViewData["UserPanelInfo"] = UserPanelInfo;

        return View(UserPanelInfo);
    }
}

