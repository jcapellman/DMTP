﻿using System;
using System.Linq;

using DMTP.lib.Common;
using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    public class WorkerController : BaseAPIController
    {
        public WorkerController(IDatabase database) : base(database) { }

        [HttpPost]
        public void Post([FromBody]Workers worker)
        {
            Database.AddUpdateWorker(worker);
        }

        [HttpDelete]
        public void DeleteHost(Guid id)
        {
            Database.DeleteWorker(id);
        }

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