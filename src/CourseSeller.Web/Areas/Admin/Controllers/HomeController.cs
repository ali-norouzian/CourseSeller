using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class HomeController : Controller
{
    private readonly IAdminService _adminService;

    public HomeController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Route("/[area]/[action]")]
    public async Task<IActionResult> Users(int pageId = 1, string filterEmail = "", string filterUserName = "")
    {
        var users = await _adminService.GetAllUsers(pageId, filterEmail, filterUserName);

        return View(users);
    }

    [Route("/[area]/[action]")]
    public async Task<IActionResult> CreateUser()
    {
        return View();
    }
}
