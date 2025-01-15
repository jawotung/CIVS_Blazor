using Application.Interfaces;
using Application.Models;
using Azure;
using DeviceDetectorNET;
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
    public class SVSController : Controller
    {
        private IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public SVSController(IConfiguration config, IUnitOfWork unitOfWork)
        {
            _config = config;
            _unitOfWork = unitOfWork;
        }


        [HttpPost("SignatureInquiry")]
        [Consumes("application/json")]
        public async Task<ReturnGenericData<ReturnSignatureInquiry>> SignatureInquiry(ParamSignatureInquiry value)
        {
            return await _unitOfWork.SVS.SignatureInquiry(value);
        }
    }
}
