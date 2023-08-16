using CourseSeller.Core.Convertors;
using CourseSeller.Core.DTOs.Account;
using CourseSeller.Core.Generators;
using CourseSeller.Core.Security;
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseSeller.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _conf;

        public AccountController(IAccountService accountService, IConfiguration conf)
        {
            _accountService = accountService;
            _conf = conf;
        }


        #region Register

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            bool errorFlag = !ModelState.IsValid;
            if (!viewModel.AcceptRules)
            {
                ModelState.AddModelError("AcceptRules", "لطفا قوانین را بپذیرید.");
                errorFlag = true;
            }
            if (await _accountService.IsExistUserName(viewModel.UserName))
            {
                ModelState.AddModelError("UserName", "نام کاربری تکراری می باشد.");
                errorFlag = true;
            }
            if (await _accountService.IsExistEmail(FixText.FixEmail(viewModel.Email)))
            {
                ModelState.AddModelError("Email", "ایمیل تکراری می باشد.");
                errorFlag = true;
            }
            if (errorFlag) { return View(viewModel); }

            // register 
            User user = new User()
            {
                UserId = CodeGenerators.Generate32ByteUniqueCode(),
                ActiveCode = CodeGenerators.Generate64ByteUniqueCode(),
                ActiveCodeGenerateDateTime = DateTime.Now,
                Email = FixText.FixEmail(viewModel.Email),
                UserName = viewModel.UserName.ToLower(),
                IsActive = false,
                Password = PasswordHelper.HashPassword(viewModel.Password),
                RegisterDateTime = DateTime.Now,
                UserAvatar = "Default.png",
            };
            user = await _accountService.AddUser(user);


            #region Send Activate Email

            //string body = _viewRender.RenderToStringAsync("Emails/_ActivateEmail", user);
            //_backgroundJobClient.Enqueue(() =>
            //    _sendEmail.Send(user.Email, "فعالسازی", body));

            #endregion


            return View("SuccessRegister", user);
        }

        #endregion


        #region Login

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            bool errorFlag = !ModelState.IsValid;

            var user = await _accountService.GetUserByEmail(viewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "ایمیل مورد نظر یافت نشد.");
                errorFlag = true;
            }
            if (errorFlag) { return View(viewModel); }

            if (PasswordHelper.VerifyPassword(viewModel.Password, user.Password))
            {
                if (!user.IsActive)
                {
                    ModelState.AddModelError("Email", "حساب کاربری شما فعال نمی باشد. ایمیل حاوی لینک فعالسازی برای شما ارسال شد.");
                    // Send new email
                    await _accountService.RevokeActiveCodeAndNewSendEmail(user);

                    return View(viewModel);
                }


                #region Login rules

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var properties = new AuthenticationProperties()
                {
                    IsPersistent = viewModel.RememberMe,
                };
                await HttpContext.SignInAsync(principal, properties);

                #endregion


                ViewData["IsSuccess"] = true;


                return View();
            }


            ModelState.AddModelError("Password", "رمز عبور نامعتبر است.");
            return View(viewModel);
        }

        #endregion


        #region Active Account

        [AllowAnonymous]
        [Route("[controller]/[action]/{userId?}/{activeCode?}")]
        public async Task<IActionResult> Activate(string userId, string activeCode)
        {
            ViewData["IsActive"] = await _accountService.ActiveAccount(userId, activeCode);

            return View();
        }

        #endregion


        #region Logout

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout(string id)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }

        #endregion
    }
}
