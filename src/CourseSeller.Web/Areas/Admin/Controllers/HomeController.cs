using CourseSeller.Core.DTOs.Admin;
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
        ViewData["Roles"] = await _adminService.GetAllRoles();

        return View();
    }

    [HttpPost]
    [Route("/[area]/[action]")]
    public async Task<IActionResult> CreateUser(CreateUserViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        var userId = await _adminService.CreateUser(viewModel);

        return RedirectToAction(nameof(Users));
    }

    [Route("/[area]/[action]/{id}")]
    public async Task<IActionResult> EditUser(string id)
    {
        ViewData["Roles"] = await _adminService.GetAllRoles();

        return View(await _adminService.GetUserInfoForUpdate(id));
    }

    [HttpPost]
    [Route("/[area]/[action]/{id}")]
    public async Task<IActionResult> EditUser(EditUserViewModel viewModel)
    {
        //if (!ModelState.IsValid)
        //    return View(viewModel);

        await _adminService.UpdateUser(viewModel);

        return Redirect(Request.Path);
    }
}
