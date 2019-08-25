using System;
using System.Linq;

using DMTP.lib.Common;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    public class WorkerController : BaseAPIController
    {
        private readonly WorkerManager _workerManager;

        public WorkerController(DatabaseManager database, Settings settings) : base(database, settings)
        {
            _workerManager = new WorkerManager(Database);
        }

        [HttpPost]
        public void Post([FromBody]Workers worker)
        {
            _workerManager.AddUpdateWorker(worker);
        }

        [HttpDelete]
        public void DeleteHost(Guid id)
        {
            _workerManager.DeleteWorker(id);
        }

        [HttpGet]
        public Jobs GetWork(string hostName)
        {
            var jobManager = new JobManager(Database);

            var job = jobManager.GetJobs()
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

            jobManager.UpdateJob(job);

            return job;
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public void UpdateWork([FromBody]Jobs job)
        {
            new JobManager(Database).UpdateJob(job);
        }
    }
}