using System;
using System.Collections.Generic;

using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    public class JobController : BaseAPIController
    {
        public JobController(IDatabase database, Settings settings) : base(database, settings) { }

        [HttpGet]
        public List<Jobs> Get() => Database.GetJobs();

        [HttpGet("{id}")]
        public Jobs Get(Guid id) => Database.GetJob(id);

        [HttpPut]
        public bool Put(Jobs item) => Database.UpdateJob(item);

        [HttpPost]
        public Guid? Post([FromBody]Jobs item) => SaveJob(item);

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            Database.DeleteJob(id);
        }
    }
}