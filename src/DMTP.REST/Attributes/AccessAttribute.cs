using System.Security.Claims;

using DMTP.lib.Auth;
using DMTP.lib.Enums;
using DMTP.lib.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Newtonsoft.Json;

namespace DMTP.REST.Attributes
{
    public class AccessAttribute : TypeFilterAttribute
    {
        public AccessAttribute(AccessSections accessSection, AccessLevels accessLevel) : base(typeof(AuthorizeActionFilter))
        {
            Arguments = new object[] { accessSection, accessLevel };
        }
    }

    public class AuthorizeActionFilter : IAuthorizationFilter
    {
        private readonly AccessSections _accessSection;
        private readonly AccessLevels _accessLevel;

        public AuthorizeActionFilter(AccessSections accessSection, AccessLevels accessLevel)
        {
            _accessSection = accessSection;
            _accessLevel = accessLevel;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var claimsIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst("ApplicationUser");

            var user = JsonConvert.DeserializeObject<ApplicationUser>(claim?.Value);

            var access = user?.Role != null && user.Role.HasPermissions(_accessSection, _accessLevel);

            if (!access)
            {
                context.Result = new RedirectToActionResult("Index", "Error", new { errorMessage = "No Access" });
            }
        }
    }
}