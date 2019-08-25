using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseAPIController : BaseController
    {
        protected BaseAPIController(DatabaseManager database, Settings settings) : base(database, settings)
        {
        }
    }
}