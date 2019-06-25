using System;

using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostController : BaseController
    {
        public HostController(IDatabase database) : base(database) { }
    
        [HttpPost]
        public void Post([FromBody]Hosts host)
        {
            Database.AddUpdateHost(host);       
        }

        [HttpDelete]
        public void DeleteHost(Guid id)
        {
            Database.DeleteHost(id);
        }
    }
}