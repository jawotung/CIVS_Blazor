using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using System.Threading.Tasks;

namespace CIVSUI.Services
{
    public class AuthorizationService
    {
        public async Task CheckAuthorization(AuthenticationStateProvider authStateProvider, NavigationManager navManager, string[] requiredRoles, string redirectRoute)
        {
            var authenticationState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authenticationState.User;

            var isAuthenticated = user.Identity.IsAuthenticated;
            var hasRequiredRole = requiredRoles.Any(role => user.IsInRole(role));

            if (!isAuthenticated || !hasRequiredRole)
            {
                navManager.NavigateTo(redirectRoute);
            }
        }
    }

}
