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
using CourseSeller.Core.Senders;
using Hangfire;

namespace CourseSeller.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ISendEmail _sendEmail;
        private readonly IViewRenderService _viewRender;
        private readonly IPasswordHelper _passwordHelper;

        public AccountController(IAccountService accountService, IBackgroundJobClient backgroundJobClient, ISendEmail sendEmail, IViewRenderService viewRender, IPasswordHelper passwordHelper)
        {
            _accountService = accountService;
            _backgroundJobClient = backgroundJobClient;
            _sendEmail = sendEmail;
            _viewRender = viewRender;
            _passwordHelper = passwordHelper;
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
                Password = await _passwordHelper.HashPassword(viewModel.Password),
                RegisterDateTime = DateTime.Now,
                UserAvatar = "Default.png",
            };
            user = await _accountService.AddUser(user);


            #region Send Activate Email

            string body = _viewRender.RenderToStringAsync("Emails/_ActivateEmail", user);
            _backgroundJobClient.Enqueue(() =>
                _sendEmail.Send(user.Email, "فعالسازی", body));

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
        public async Task<IActionResult> Login(LoginViewModel viewModel, [FromQuery] string? ReturnUrl)
        {
            bool errorFlag = !ModelState.IsValid;

            var user = await _accountService.GetUserByEmail(viewModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "ایمیل مورد نظر یافت نشد.");
                errorFlag = true;
            }
            if (errorFlag) { return View(viewModel); }

            if (await _passwordHelper.VerifyPassword(viewModel.Password, user.Password))
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

                ViewData["ReturnUrl"] = !string.IsNullOrEmpty(ReturnUrl) ? ReturnUrl : "/UserPanel";

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


        #region Forgot Password

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            bool errorFlag = false;
            if (!ModelState.IsValid)
                errorFlag = true;

            string fixedEmail = FixText.FixEmail(viewModel.Email);
            User user = await _accountService.GetUserByEmail(fixedEmail);
            if (user == null)
            {
                ModelState.AddModelError("Email", "کاربری یافت نشد.");
                errorFlag = true;
            }

            if (errorFlag) { return View(viewModel); }

            await _accountService.RevokeActiveCodeAndNewSendEmail(user, "Emails/_ForgotPassword", "بازیابی حساب کاربری");
            ViewData["IsSuccess"] = true;

            return View();
        }


        #endregion


        #region Reset Password

        [AllowAnonymous]
        [HttpGet]
        [Route("[controller]/[action]/{userId?}/{activeCode?}")]
        public IActionResult ResetPassword(string userId, string activeCode)
        {
            return View(new ResetPasswordViewModel()
            {
                UserId = userId,
                ActiveCode = activeCode,
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("[controller]/[action]/{userId?}/{activeCode?}")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            bool errorFlag = !ModelState.IsValid;

            User user = await _accountService.GetUserByActiveCode(viewModel.UserId, viewModel.ActiveCode);
            if (user == null)
            {
                ModelState.AddModelError("Password", "لینک شما منقضی شده است. لینک جدید برای شما ارسال شد.");
                errorFlag = true;
            }

            if (errorFlag) { return View(viewModel); }

            // This will reset it and expire used token.
            await _accountService.ResetPassword(user, viewModel.Password);

            string body = _viewRender.RenderToStringAsync("Emails/_PasswordChanged", user);
            _backgroundJobClient.Enqueue(() =>
                _sendEmail.Send(user.Email, "تغییر رمز عبور", body));

            TempData["ResetPasswordIsSuccess"] = true;


            return RedirectToAction(nameof(Login));
        }

        #endregion
    }
}
