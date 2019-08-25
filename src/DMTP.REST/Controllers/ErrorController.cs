using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class ErrorController : BaseController
    {
        public ErrorController(DatabaseManager database, Settings settings) : base(database, settings)
        {
        }

        public IActionResult Index(string errorMessage) => View("Index", errorMessage);
    }
}