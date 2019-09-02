using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers;

using DMTP.REST.Models.Workers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class WorkersController : BaseController
    {
        private string ComputeRegistrationKey() => CurrentSettings.DeviceKeyPassword;

        public WorkersController(DatabaseManager database, Settings settings) : base(database, settings)
        {
        }

        public IActionResult Index() => View(new WorkerListingModel
        {
            WorkersListing = new WorkerManager(Database).GetWorkers(),
            RegistrationKey = ComputeRegistrationKey(),
            WebServiceURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/"
        });
    }
}