using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Handlers
{
    public class RefererValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RefererValidationMiddleware(
            RequestDelegate next, IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            var referer = context.Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(referer) && referer.StartsWith(_config["CIVSUIUrl"]))
            {
                await _next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: Invalid referer");
            }
        }
    }
}
