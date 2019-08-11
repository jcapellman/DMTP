﻿using DMTP.lib.Databases.Base;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    public class WorkersController : BaseController
    {
        public WorkersController(IDatabase database) : base(database)
        {
        }

        public IActionResult Index() => View(Database.GetHosts());
    }
}