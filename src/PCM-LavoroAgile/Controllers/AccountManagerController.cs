using Domain.Model.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PCM_LavoroAgile.Models.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AccountManagerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountManagerController(SignInManager<AppUser> signInManager, ILogger<AccountController> logger, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult CreateUser()
        {
            ViewData["roles"] = _roleManager.Roles.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel registerModel, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            var role = await _roleManager.FindByIdAsync(registerModel.Input.UserRole);
            registerModel.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = registerModel.Input.Email, Email = registerModel.Input.Email, FullName = registerModel.Input.FullName };
                IdentityResult result;
                if (String.IsNullOrWhiteSpace(registerModel.Input.Password))
                    result = await _userManager.CreateAsync(user);
                else
                    result = await _userManager.CreateAsync(user, registerModel.Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if(role != null)
                        await _userManager.AddToRoleAsync(user, role.Name);

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = registerModel.Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    return LocalRedirect(returnUrl);
                    //}

                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["roles"] = _roleManager.Roles.ToList();
            return View();
        }

        public IActionResult Claims()
        {
            return View();
        }
    }
}
