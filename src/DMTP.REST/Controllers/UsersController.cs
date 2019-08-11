using DMTP.lib.Databases.Base;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    public class UsersController : BaseController
    {
        public UsersController(IDatabase database) : base(database)
        {
        }

        public IActionResult Index() => View(Database.GetUsers());
    }
}