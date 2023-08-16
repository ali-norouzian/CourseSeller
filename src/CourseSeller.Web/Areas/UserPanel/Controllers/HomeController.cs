using CourseSeller.Core.Convertors;
using CourseSeller.Core.DTOs.UserPanel;
using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
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

    public async Task<IActionResult> EditProfile()
    {
        var oldData = await _userPanelService.GetDataForEditUserProfile(User.Identity.Name);
        return View(oldData);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(EditProfileViewModel viewModel)
    {
        // We can get old value of fields in hidden forms for -
        // - checking before quering db but now we don't need it.
        bool errorFlag = !ModelState.IsValid;
        bool userNameOrEmailChangedFlag = false;
        if (viewModel.OldUserName != viewModel.UserName)
        {
            userNameOrEmailChangedFlag = true;
            TempData["EditedProfileUserName"] = true;
            if (await _accountService.IsExistUserName(viewModel.UserName))
            {
                ModelState.AddModelError("UserName", "نام کاربری تکراری می باشد.");
                errorFlag = true;
            }
        }

        if (viewModel.OldEmail != viewModel.Email)
        {
            userNameOrEmailChangedFlag = true;
            TempData["EditedProfileEmail"] = true;
            if (await _accountService.IsExistEmail(FixText.FixEmail(viewModel.Email)))
            {
                ModelState.AddModelError("Email", "ایمیل تکراری می باشد.");
                errorFlag = true;
            }
        }
        if (viewModel.UserAvatar != null)
            if (!_userPanelService.ImageHasValidExtension(viewModel.UserAvatar.FileName))
            {
                ModelState.AddModelError("UserAvatar", "عکس آپلود شده معتبر نمی باشد.");
                errorFlag = true;
            }

        // Error for duplicated usages
        if (errorFlag)
        {
            // Fill SideBarViewModel. It was null there.
            var sideBarViewModel = await _userPanelService.GetSideBarData(viewModel.OldUserName);
            viewModel.SideBarViewModel = sideBarViewModel;

            // Fill OldUserName & email with true value in the db not that changed there
            // If it not change this accure bug when click two time with validation error for change fields.
            viewModel.UserName = viewModel.OldUserName;
            viewModel.Email = viewModel.OldEmail;

            return View(viewModel);
        }

        // Go for this when have not error
        await _userPanelService.EditProfile(User.Identity.Name, viewModel);

        ViewData["IsSuccess"] = true;

        // Logout for setting cookies with new username 
        if (userNameOrEmailChangedFlag)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/Account/Login");
        }

        return RedirectToAction(nameof(EditProfile));
    }
}

