using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class JobsController : BaseController
    {
        public JobsController(IDatabase database, Settings settings) : base(database, settings)
        {
        }

        public IActionResult Index() => View(Database.GetJobs());
    }
}