using DMTP.lib.dal.Databases.Base;
using DMTP.lib.dal.Databases.Tables;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseAPIController : BaseController
    {
        protected BaseAPIController(IDatabase database, Settings settings) : base(database, settings)
        {
        }
    }
}