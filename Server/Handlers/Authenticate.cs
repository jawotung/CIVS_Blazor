using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YamlDotNet.Core.Tokens;
using Application.Interfaces;

namespace WebAPI.Handlers
{
    public class Authenticate : AuthorizeAttribute, IAuthorizationFilter
    {
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            // Perform the standard authorization check
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var apiService = context.HttpContext.RequestServices.GetRequiredService<IApiServiceRepository>();

            var isValid = await IsValidUser(user, token, apiService);
            if (!isValid)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }

        private async Task<bool> IsValidUser(System.Security.Claims.ClaimsPrincipal user, string token, IApiServiceRepository apiService)
        {

            var nameIdentifier = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var result = true; //await apiService.Authenticate(nameIdentifier, token);
            return result;
        }
    }
}
