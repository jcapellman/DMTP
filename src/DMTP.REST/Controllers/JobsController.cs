using System.Linq;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class JobsController : BaseController
    {
        private readonly IStringLocalizer<JobsController> _localizer;

        public JobsController(DatabaseManager database, Settings settings, IStringLocalizer<JobsController> localizer) : base(database, settings)
        {
            _localizer = localizer;
        }

        public IActionResult Index() => View(new JobManager(Database).GetJobs().OrderByDescending(a => a.SubmissionTime).ToList());
    }
}