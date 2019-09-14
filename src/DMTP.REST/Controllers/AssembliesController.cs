using System;
using System.IO;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Enums;
using DMTP.lib.dal.Manager;

using DMTP.lib.Managers;

using DMTP.REST.Attributes;
using DMTP.REST.Models.Assemblies;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class AssembliesController : BaseController
    {
        private readonly IStringLocalizer<AssembliesController> _localizer;

        public AssembliesController(DatabaseManager database, Settings settings, IStringLocalizer<AssembliesController> localizer) : base(database, settings)
        {
            _localizer = localizer;
        }

        [HttpPost]
        [Access(AccessSections.ASSEMBLIES, AccessLevels.EDIT)]
        public IActionResult UploadAssembly(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);

                var result = new AssemblyManager(Database).UploadAssembly(ms.ToArray(), file.FileName);

                return RedirectToAction("Index", !result ? new { actionMessage = $"{_localizer["InvalidAssembly"]} ({file.FileName})" } : new {actionMessage = $"{_localizer["SuccessfullyUpload"]} {file.FileName}"});
            }
        }

        [HttpGet]
        [Access(AccessSections.ASSEMBLIES, AccessLevels.FULL)]
        public IActionResult DeleteAssembly(Guid id) =>
            RedirectToAction("Index",
                new AssemblyManager(Database).DeleteAssembly(id)
                    ? new {actionMessage = _localizer["SuccessfullyDeletedAssembly"] }
                    : new {actionMessage = _localizer["FailedToDeleteAssembly"] });

        [Access(AccessSections.ASSEMBLIES, AccessLevels.VIEW_ONLY)]
        public IActionResult Index(string actionMessage = null)
        {
            var model = new AssemblyDashboardModel()
            {
                AssembliesList = new AssemblyManager(Database).GetUploadedAssembliesList(),
                ActionMessage = actionMessage
            };

            return View(model);
        }
    }
}