using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
 
namespace CIVSUI.States
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string LocalStorageKey = "auth";
        public CustomAuthenticationStateProvider(ILocalStorageService localStorageService)
        {
            this.localStorageService = localStorageService;
        }


        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        private readonly ILocalStorageService localStorageService;


        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await localStorageService.GetItemAsStringAsync(LocalStorageKey)!;
            if (string.IsNullOrEmpty(token))
                return await Task.FromResult(new AuthenticationState(anonymous));

            var name = GetClaims(token);
            if (string.IsNullOrEmpty(name))
                return await Task.FromResult(new AuthenticationState(anonymous));

            var claims = SetClaimPrincipal(name);
            if (claims is null)
                return await Task.FromResult(new AuthenticationState(anonymous));
            else
                return await Task.FromResult(new AuthenticationState(claims));
        }

        public static ClaimsPrincipal SetClaimPrincipal(string name)
        {
            if (name is null) return new ClaimsPrincipal();
            return new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new(ClaimTypes.Name, name!)
                    ], "JwtAuth"));
        }

        private string GetClaims(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken)) return null;

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            var nameIdentifierClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return nameIdentifierClaim;
        }

        public async Task UpdateAuthenticationState(string jwtToken)
        {

            var claims = new ClaimsPrincipal();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                var name = GetClaims(jwtToken);
                if (string.IsNullOrEmpty(name))
                    return;

                var setClaims = SetClaimPrincipal(name);
                if (setClaims is null)
                    return;

                await localStorageService.SetItemAsStringAsync(LocalStorageKey, jwtToken);


            }
            else
            {
                await localStorageService.RemoveItemAsync(LocalStorageKey);
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claims)));
        }



    }
}