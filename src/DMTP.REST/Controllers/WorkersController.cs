using DMTP.lib.Databases.Base;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class WorkersController : BaseController
    {
        public WorkersController(IDatabase database) : base(database)
        {
        }

        public IActionResult Index() => View(Database.GetWorkers());
    }
}