using Application.Interfaces;
using Application.Models;
using Azure;
using DeviceDetectorNET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WebAPI.Handlers;
using YamlDotNet.Core.Tokens;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IConfiguration config, IUnitOfWork unitOfWork)
        {
            _config = config;
            _unitOfWork = unitOfWork;
        }


        [HttpPost("ValidateCredentials")]
        [Consumes("application/json")]
        public async Task<ActionResult<ReturnGenericData<ReturnLoginCredentials>>> ValidateCredentials(ParamLoginCredentials value)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress;
            var userAgent = HttpContext.Request.Headers["User-Agent"];
            value.IpAddress = ipAddress.ToString();
            value.UserAgent = userAgent.ToString();
            var results = await _unitOfWork.Auth.ValidateCredentials(value);
            object response;
            if (results != null)
            {
                if (results.StatusCode == "00")
                {
                    
                    var deviceDetector = new DeviceDetector(userAgent);
                    deviceDetector.Parse();
                    var remoteInfo = new
                    {
                        ipAddress = ipAddress.ToString(),
                        userAgent = userAgent
                    };

                    var jwtToken = await GenerateToken(results, remoteInfo);
                    response = jwtToken;
                }
                else
                {
                    response = new { StatusCode = results.StatusCode, StatusMessage = results.StatusMessage };
                }

                return Ok(response);
            }
            response = new { StatusCode = "01", StatusMessage = "Invalid credentials." };
            return Ok(response);
        }

        [Authorize]
        [HttpPost("TagInactiveUsers")]
        [Consumes("application/json")]
        public async Task<ReturnGenericStatus> TagInactiveUsers(int? mode)
        {
            return await _unitOfWork.Auth.TagInactiveUsers(mode);
        }

        [Authorize]
        [HttpPost("ClearAllSession")]
        [Consumes("application/json")]
        public async Task<ReturnGenericStatus> ClearAllSession()
        {
            return await _unitOfWork.Auth.ClearAllSession();
        }

        [HttpPost("Logout")]
        [Consumes("application/json")]
        public async Task<ReturnGenericStatus> AuthLogout(string? sMode)
        {
            return await _unitOfWork.Auth.AuthLogout(sMode);
        }


        private async Task<ReturnLoginCredentials> GenerateToken(ReturnLoginCredentials value, object remoteInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userClaimsOthers = new[] {
                value.UserTypeDesc, //UserType
                value.BranchOfAssignmentCode, //BranchCode
                value.BranchOfAssignmentDesc, //Branch
                value.GroupingDesc, //Group
                value.MenuDesc, //Menu
                value.LastLoginDate.ToString(), //LastLoginDate
                value.BCPBranch.BranchBuddyCode, //BuddyCode
                value.BCPBranch.BranchBuddyDesc //BuddyBranch
            };

            var userClaims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier,value.UserId.ToString()),
                    new Claim(ClaimTypes.Name, value.DisplayName!),
                    new Claim(ClaimTypes.Email, value.DisplayName !),
                    new Claim(ClaimTypes.Role, value.UserTypeDesc !),
                    new Claim(ClaimTypes.GivenName, string.Join(", ", userClaimsOthers) !),
                    new Claim(ClaimTypes.DateOfBirth, value.LastLoginDate.ToString() !),
            };

            int tokenValidityInMinutes = int.Parse(_config["Jwt:TokenValidityInMinutes"]);
            int refreshTokenValidityInMinutes = int.Parse(_config["Jwt:RefreshTokenValidityInMinutes"]);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: userClaims,
                expires: GetCurrentDateTime().AddMinutes(tokenValidityInMinutes),
                signingCredentials: credentials
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = RefreshTokenGeneration(30);
            dynamic info = remoteInfo;

            var saveAuthenticationLogin = new ParamSaveApiAuthentication()
            {
                Type = "JWT",
                UserId = value.UserId,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = GetCurrentDateTime().AddMinutes(tokenValidityInMinutes),
                RefreshTokenExpiry = GetCurrentDateTime().AddMinutes(refreshTokenValidityInMinutes),
                IpAddress = info.ipAddress,
                UserAgent = info.userAgent
            };

            //await _unitOfWork.Auth.SaveApiAuthentication(saveAuthenticationLogin);

            return new ReturnLoginCredentials()
            {
                StatusCode = "00",
                StatusMessage = "SUCCESS",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresIn = tokenValidityInMinutes.ToString(),
                RefreshTokenExpiresIn = refreshTokenValidityInMinutes.ToString(),
            };
        }

        private static DateTime GetCurrentDateTime()
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo phTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
            DateTime phTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, phTimeZone);
            return phTime;
        }

        private static string RefreshTokenGeneration(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz_";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
