using System;
using System.IO;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Enums;
using DMTP.lib.dal.Manager;

using DMTP.lib.Enums;
using DMTP.lib.Managers;

using DMTP.REST.Attributes;
using DMTP.REST.Models.Assemblies;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class AssembliesController : BaseController
    {
        public AssembliesController(DatabaseManager database, Settings settings) : base(database, settings)
        {
        }

        [HttpPost]
        [Access(AccessSections.ASSEMBLIES, AccessLevels.EDIT)]
        public IActionResult UploadAssembly(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);

                var result = new AssemblyManager(Database).UploadAssembly(ms.ToArray(), file.FileName);

                return RedirectToAction("Index", !result ? new { actionMessage = $"Invalid assembly ({file.FileName})" } : new {actionMessage = $"Successfully upload {file.FileName}"});
            }
        }

        [HttpDelete]
        [Access(AccessSections.ASSEMBLIES, AccessLevels.FULL)]
        public IActionResult DeleteAssembly(Guid id) =>
            RedirectToAction("Index",
                new AssemblyManager(Database).DeleteAssembly(id)
                    ? new {actionMessage = "Successfully deleted assembly"}
                    : new {actionMessage = "Failed to delete assembly"});

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