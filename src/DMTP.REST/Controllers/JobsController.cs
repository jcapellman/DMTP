using DMTP.lib.dal.Databases.Base;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.Managers;

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

        public IActionResult Index() => View(new JobManager(Database).GetJobs());
    }
}