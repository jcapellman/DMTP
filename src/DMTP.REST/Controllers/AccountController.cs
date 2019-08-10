using System.Collections.Generic;
using System.Security.Claims;

using DMTP.lib.Databases.Base;
using DMTP.lib.Helpers;
using DMTP.REST.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IDatabase _database;

        public AccountController(IDatabase database)
        {
            _database = database;
        }

        public IActionResult Index() => View(new LoginViewModel());

        [HttpPost]
        public IActionResult AttemptLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Please try again";

                return View("Index", model);
            }

            var userGuid = _database.GetUser(model.Username, model.Password.ToSHA1());

            if (userGuid == null)
            {
                model.ErrorMessage = "Username and or Password are incorrect";

                return View("Index", model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userGuid.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties
            {
                IsPersistent = true
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).Wait();

            return RedirectToAction("Index", "Home");
        }
    }
}