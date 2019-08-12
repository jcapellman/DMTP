﻿using System;
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

        public IActionResult Index(string ReturnUrl = "") => View(new LoginViewModel());

        public IActionResult Create() => View(new CreateUserModel());

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();

            return RedirectToAction("Index", "Account");
        }

        private IActionResult Login(Guid userGuid)
        {
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

                return View("Index", model);
            }

            var userGuid = _database.GetUser(model.EmailAddress, model.Password.ToSHA1());

            _database.RecordLogin(userGuid, model.EmailAddress, Request.HttpContext.Connection.RemoteIpAddress.ToString(), userGuid.HasValue);

            if (userGuid != null)
            {
                return Login(userGuid.Value);
            }

            model.ErrorMessage = "EmailAddress and or Password are incorrect";

            return View("Index", model);
        }
    }
}