using DMTP.lib.dal.Databases.Tables;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DMTP.REST.Filters
{
    public class APIAuthorize : ActionFilterAttribute
    {
        private readonly Settings _settings;

        public APIAuthorize(Settings settings)
        {
            _settings = settings;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var value))
            {
                context.Result = new ForbidResult();
            }

            if (value.ToString().Split(' ')[1] != _settings.DeviceKeyPassword)
            {
                context.Result = new ForbidResult();
            }

            base.OnActionExecuting(context);
        }
    }
}