using CourseSeller.Core.DTOs.Admin;
using CourseSeller.Core.Security;
using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class UserController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IUserPanelService _userPanelService;

    public UserController(IAdminService adminService, IUserPanelService userPanelService)
    {
        _adminService = adminService;
        _userPanelService = userPanelService;
    }

    [PermissionChecker(PermissionCheckerAttribute.PanelManagementId)]
    [Route("/[area]")]
    public IActionResult Index()
    {
        return View();
    }

    [PermissionChecker(PermissionCheckerAttribute.UsersManagementId)]
    [Route("/[area]/[action]")]
    public async Task<IActionResult> Users(int pageId = 1, string filterEmail = "", string filterUserName = "")
    {
        var users = await _adminService.GetAllUsers(pageId, filterEmail, filterUserName);

        return View(users);
    }


    [PermissionChecker(PermissionCheckerAttribute.DeleteUserId)]
    [Route("/[area]/Users/Deleted")]
    public async Task<IActionResult> DeletedUsers(int pageId = 1, string filterEmail = "", string filterUserName = "")
    {
        var users = await _adminService.GetAllDeletedUsers(pageId, filterEmail, filterUserName);

        return View(users);
    }

    [PermissionChecker(PermissionCheckerAttribute.CreateUserId)]
    [Route("/[area]/Users/Create")]
    public async Task<IActionResult> CreateUser()
    {
        ViewData["Roles"] = await _adminService.GetAllRoles();

        return View();
    }

    [PermissionChecker(PermissionCheckerAttribute.CreateUserId)]
    [HttpPost]
    [Route("/[area]/Users/Create")]
    public async Task<IActionResult> CreateUser(CreateUserViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        var userId = await _adminService.CreateUser(viewModel);

        return RedirectToAction(nameof(Users));
    }

    [PermissionChecker(PermissionCheckerAttribute.EditUserId)]
    [Route("/[area]/Users/Update/{id}")]
    public async Task<IActionResult> EditUser(string id)
    {
        ViewData["Roles"] = await _adminService.GetAllRoles();

        return View(await _adminService.GetUserInfoForUpdate(id));
    }

    [PermissionChecker(PermissionCheckerAttribute.EditUserId)]
    [HttpPost]
    [Route("/[area]/Users/Update/{id}")]
    public async Task<IActionResult> EditUser(EditUserViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        await _adminService.UpdateUser(viewModel);

        return Redirect(Request.Path);
    }

    [PermissionChecker(PermissionCheckerAttribute.DeleteUserId)]
    [Route("/[area]/Users/Delete/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        ViewData["UserId"] = id;

        return View(await _userPanelService.GetUserInfoById(id));
    }

    [PermissionChecker(PermissionCheckerAttribute.DeleteUserId)]
    [HttpPost]
    [Route("/[area]/Users/Delete/{id}")]
    public async Task<IActionResult> DeleteUser(EditUserViewModel viewModel)
    {
        await _adminService.SoftDeleteUser(viewModel.UserId);

        return RedirectToAction(nameof(DeletedUsers));
    }
}
