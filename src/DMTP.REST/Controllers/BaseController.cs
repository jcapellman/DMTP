using System;
using System.Linq;

using DMTP.lib.Common;
using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IDatabase Database;

        protected BaseController(IDatabase database)
        {
            Database = database;
        }

        protected Guid? SaveJob(Jobs job)
        {
            job.ID = Guid.NewGuid();
            job.SubmissionTime = DateTime.Now;

            var hosts = Database.GetHosts();

            if (hosts == null)
            {
                return null;
            }

            if (hosts.Any())
            {
                var jobs = Database.GetJobs().Where(a => !a.Completed).ToList();

                foreach (var host in hosts)
                {
                    if (jobs.Any(a => a.AssignedHost == host.Name))
                    {
                        continue;
                    }

                    job.AssignedHost = host.Name;

                    break;
                }

                if (string.IsNullOrEmpty(job.AssignedHost))
                {
                    job.AssignedHost = Constants.UNASSIGNED_JOB;
                }
            }
            else
            {
                job.AssignedHost = Constants.UNASSIGNED_JOB;
            }

            if (Database.AddJob(job))
            {
                return job.ID;
            }

            return null;
        }
    }
}