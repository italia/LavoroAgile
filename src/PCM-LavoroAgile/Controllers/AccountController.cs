using Domain.Model.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PCM_LavoroAgile.Extensions;
using PCM_LavoroAgile.Models.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly LdapSignInService? _ldapSignInManager;
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<AppUser> signInManager, IAccountService accountService, ILogger<AccountController> logger, LdapSignInService ldapSignInManager = null)
        {
            _signInManager = signInManager;
            _accountService = accountService;
            _ldapSignInManager = ldapSignInManager;
            _logger = logger;
        }

        [ResponseCache(NoStore = true, VaryByHeader = "None", Duration = 0)]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginModel, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            loginModel.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await this.ExecuteLogin(loginModel);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    return LocalRedirect(returnUrl);
                }
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = loginModel.Input.RememberMe });
                //}
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning("User account locked out.");
                //    return RedirectToPage("./Lockout");
                //}
                else
                {
                    //ModelState.AddModelError(string.Empty, "Login fallito!");
                    TempData.SendNotification(NotificationType.Error, "Login fallito!");
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        /// <summary>
        /// Effettua la login.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        private async Task<Microsoft.AspNetCore.Identity.SignInResult> ExecuteLogin(LoginViewModel loginViewModel)
        {
            if (loginViewModel.Input.LoginAD)
            {
                return await _ldapSignInManager.PasswordSignInAsync(loginViewModel.Input.Email, loginViewModel.Input.Password, loginViewModel.Input.RememberMe);
            }
            return await _signInManager.PasswordSignInAsync(loginViewModel.Input.Email, loginViewModel.Input.Password, loginViewModel.Input.RememberMe, lockoutOnFailure: false);

        }

        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            await _signInManager.SignOutAsync();

            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return LocalRedirect(returnUrl);
            }
        }

        /// <summary>
        /// Mostra la pagina di cambio password.
        /// </summary>
        /// <returns>Pagina di cambio password.</returns>
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Effettua il cambio password.
        /// </summary>
        /// <param name="changePassword">Richiesta di cambio password.</param>
        /// <returns>Pagina di login</returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.ChangePassword(User.Identity.Name, changePassword.CurrentPassword, changePassword.NewPassword, changePassword.ConfirmPassword);
                if (result.Succeeded)
                {
                    // Effettua la signout e ridirige verso la login.
                    await _signInManager.SignOutAsync();
                    return RedirectPermanent(nameof(Login));

                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData.SendNotification(NotificationType.Error, string.IsNullOrWhiteSpace(errors) ? "Login fallito!" : errors);
                    return View(changePassword);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(changePassword);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
