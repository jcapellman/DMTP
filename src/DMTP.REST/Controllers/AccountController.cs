﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;

using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Helpers;
using DMTP.REST.Auth;
using DMTP.REST.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace DMTP.REST.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IDatabase _database;
        private readonly Settings _settings;

        public AccountController(IDatabase database, Settings settings)
        {
            _database = database;
            _settings = settings;
        }

        public IActionResult Index(string ReturnUrl = "") => View(new LoginViewModel
        {
            CurrentSettings = _settings
        });

        public IActionResult Create()
        {
            if (!_settings.AllowNewUserCreation)
            {
                return RedirectToAction("Index", "Account");
            }

            return View(new CreateUserModel());
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();

            return RedirectToAction("Index", "Account");
        }

        private IActionResult Login(Guid userGuid)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userGuid.ToString()),
                new Claim("ApplicationUser", JsonConvert.SerializeObject(new ApplicationUser()))
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties
            {
                IsPersistent = true
            };

            Thread.CurrentPrincipal = principal;

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).Wait();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult AttemptCreate(CreateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Please try again";
                
                return View("Create", model);
            }

            var userGuid = _database.CreateUser(model.EmailAddress, model.FirstName, model.LastName, model.Password.ToSHA1());

            _database.RecordLogin(userGuid, model.EmailAddress, Request.HttpContext.Connection.RemoteIpAddress.ToString(), userGuid.HasValue);

            if (userGuid != null)
            {
                return Login(userGuid.Value);
            }

            model.ErrorMessage = "EmailAddress already exists, try again";

            model.Password = string.Empty;
            model.EmailAddress = string.Empty;
            model.FirstName = string.Empty;
            model.LastName = string.Empty;

            return View("Create", model);
        }

        [HttpPost]
        public IActionResult AttemptLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Please try again";

                model.CurrentSettings = _settings;

                return View("Index", model);
            }

            var userGuid = _database.GetUser(model.EmailAddress, model.Password.ToSHA1());

            _database.RecordLogin(userGuid, model.EmailAddress, Request.HttpContext.Connection.RemoteIpAddress.ToString(), userGuid.HasValue);

            if (userGuid != null)
            {
                return Login(userGuid.Value);
            }

            model.ErrorMessage = "Email Address and or Password are incorrect";
            model.CurrentSettings = _settings;

            return View("Index", model);
        }
    }
}