using DMTP.lib.Databases.Base;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseAPIController : BaseController
    {
        protected BaseAPIController(IDatabase database) : base(database)
        {
        }
    }
}