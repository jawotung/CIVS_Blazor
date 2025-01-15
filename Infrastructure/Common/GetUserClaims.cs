using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    public class GetUserClaims
    {
        private readonly IHttpContextAccessor _httpContext;

        public GetUserClaims(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public UserClaims GetClaims()
        {
            var httpContext = _httpContext.HttpContext;
            var userClaims = new UserClaims();
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();
                var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                if (httpContext.User.Identity.IsAuthenticated)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.ReadToken(accessToken) as JwtSecurityToken;
                    var nameIdentifier = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    var displayName = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    var userClaimsOthers = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
                    var userClaimsOthersArray = userClaimsOthers.Split(", ");
                    userClaims = new UserClaims()
                    {
                        UserID = nameIdentifier,
                        DisplayName = displayName,
                        UserType = userClaimsOthersArray[0],
                        BranchCode = userClaimsOthersArray[1],
                        Branch = userClaimsOthersArray[2],
                        Group = userClaimsOthersArray[3],
                        Menu = userClaimsOthersArray[4],
                        LastLoginDate = userClaimsOthersArray[5],
                        BuddyCode = userClaimsOthersArray[6],
                        BuddyBranch = userClaimsOthersArray[7],
                    };
                }
            }
            return userClaims;
        }
    }
}
