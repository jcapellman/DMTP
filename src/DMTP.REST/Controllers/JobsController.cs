using DMTP.lib.Databases.Base;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class JobsController : BaseController
    {
        public JobsController(IDatabase database) : base(database)
        {
        }

        public IActionResult Index() => View(Database.GetJobs());
    }
}