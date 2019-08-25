using System;
using System.Collections.Generic;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    public class JobController : BaseAPIController
    {
        private readonly JobManager _jobManager;

        public JobController(DatabaseManager database, Settings settings) : base(database, settings)
        {
            _jobManager = new JobManager(Database);
        }

        [HttpGet]
        public List<Jobs> Get() => _jobManager.GetJobs();

        [HttpGet("{id}")]
        public Jobs Get(Guid id) => _jobManager.GetJob(id);

        [HttpPut]
        public bool Put(Jobs item) => _jobManager.UpdateJob(item);

        [HttpPost]
        public Guid? Post([FromBody]Jobs item) => SaveJob(item);

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            _jobManager.DeleteJob(id);
        }
    }
}