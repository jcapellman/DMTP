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
            var jobs = Database.GetJobs().Where(a => !a.Completed).ToList();

            var assignedJob = jobs.FirstOrDefault(a => a.AssignedHost == hostName);

            if (assignedJob != null)
            {
                return assignedJob;
            }

            var unassignedJob = jobs.FirstOrDefault(a => a.AssignedHost == Constants.UNASSIGNED_JOB);

            // No jobs available
            if (unassignedJob == null)
            {
                return null;
            }

            // Assign the first unassigned job to the hostName
            unassignedJob.AssignedHost = hostName;

            Database.UpdateJob(unassignedJob);

            return unassignedJob;
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public void UpdateWork([FromBody]Jobs job)
        {
            Database.UpdateJob(job);
        }
    }
}