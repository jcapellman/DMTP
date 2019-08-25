using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class WorkersController : BaseController
    {
        public WorkersController(DatabaseManager database, Settings settings) : base(database, settings)
        {
        }

        public IActionResult Index() => View(new WorkerManager(Database).GetWorkers());
    }
}