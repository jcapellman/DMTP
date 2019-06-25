using System.Linq;

using DMTP.lib.Common;
using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : BaseController
    {
        public WorkerController(IDatabase database) : base(database) { }
    
        [HttpGet]
        public Jobs GetWork(string hostName)
        {
            var job = Database.GetJobs()
                .FirstOrDefault(a => !a.Completed && (a.AssignedHost == hostName || a.AssignedHost == Constants.UNASSIGNED_JOB));

            if (job != null && job.AssignedHost == hostName)
            {
                return job;
            }

            if (job == null || job.AssignedHost != Constants.UNASSIGNED_JOB)
            {
                return null;
            }

            // Assign the first unassigned job to the hostName
            job.AssignedHost = hostName;

            Database.UpdateJob(job);

            return job;
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public void UpdateWork([FromBody]Jobs job)
        {
            Database.UpdateJob(job);
        }
    }
}