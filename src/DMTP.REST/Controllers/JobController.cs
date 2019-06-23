using System;
using System.Collections.Generic;
using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : BaseController
    {
        public JobController(IDatabase database) : base(database) { }

        [HttpGet]
        public List<Jobs> Get() => Database.GetJobs();

        [HttpGet("{id}")]
        public Jobs Get(Guid id) => Database.GetJob(id);

        [HttpPut]
        public bool Put(Jobs item) => Database.UpdateJob(item);

        [HttpPost]
        public Guid Post([FromBody]Jobs item) => SaveJob(item);

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            Database.DeleteJob(id);
        }
    }
}