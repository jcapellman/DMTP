using System.Linq;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class JobsController : BaseController
    {
        public JobsController(DatabaseManager database, Settings settings) : base(database, settings)
        {
        }

        public IActionResult Index() => View(new JobManager(Database).GetJobs().OrderByDescending(a => a.SubmissionTime).ToList());
    }
}