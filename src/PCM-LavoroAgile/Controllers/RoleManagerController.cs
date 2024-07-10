using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace PCM_LavoroAgile.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RoleManagerController : Controller
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public RoleManagerController(ILogger<AccountController> logger, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public IActionResult CreateRole()
        {
            return View(new IdentityRole<Guid>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRole<Guid> role, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            await _roleManager.CreateAsync(role);
            //return RedirectToAction("Index");

            _logger.LogInformation("Role created.");

            return LocalRedirect(returnUrl);
        }
    }
}
