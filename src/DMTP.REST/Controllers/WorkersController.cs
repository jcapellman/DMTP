using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class WorkersController : BaseController
    {
        public WorkersController(IDatabase database, Settings settings) : base(database, settings)
        {
        }

        public IActionResult Index() => View(Database.GetWorkers());
    }
}