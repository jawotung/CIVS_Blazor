using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using YamlDotNet.Core.Tokens;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class InwardClearingChequeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public InwardClearingChequeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpGet("GetStatusList")]
        public ReturnGenericDropdown GetStatusList()
        {
            return _unitOfWork.InwardClearingCheque.GetStatusList();
        }

        [HttpGet("GetICCList")]
        public async Task<ReturnGenericData<PaginatedOutput<InwardClearingChequeDetailsModel>>> GetICCList(string? key, int? page, [FromQuery] ParamInwardClearingChequeGetList value)
        {
            var result = await _unitOfWork.InwardClearingCheque.GetICCList(key, page, value);
            return result;
        }

        [HttpGet("GetICCDetails")]
        public async Task<ReturnGenericData<CheckDetailModel>> GetICCDetails(int id)
        {
            var result = await _unitOfWork.InwardClearingCheque.GetICCDetails(id);
            return result;
        }

        [HttpGet("GetCheckImageDetails")]
        public async Task<ReturnGenericData<ReturnCheckImageDetailTransaction>> GetCheckImageDetails(string sKey)
        {
            var result = await _unitOfWork.InwardClearingCheque.GetCheckImageDetails(sKey);
            return result;
        }

        [HttpGet("GetViewSignatures")]
        public async Task<ReturnGenericData<ReturnViewSignatures>> GetViewSignatures(string accountNo)
        {
            var result = await _unitOfWork.InwardClearingCheque.GetViewSignatures(accountNo);
            return result;
        }

        [HttpGet("GetUserAllowedAccess")]
        public async Task<ReturnGenericList<SelectListItem>> GetUserAllowedAccess(string? key, string amount)
        {
            var result = await _unitOfWork.InwardClearingCheque.GetUserAllowedAccess(key, amount);
            return result;
        }

        [HttpGet("GetReasonList")]
        public async Task<ReturnGenericList<SelectListItem>> GetReasonList()
        {
            var result = await _unitOfWork.InwardClearingCheque.GetReasonList();
            return result;
        }

        [HttpGet("GetOfficerList")]
        public async Task<ReturnGenericList<SelectListItem>> GetOfficerList()
        {
            var result = await _unitOfWork.InwardClearingCheque.GetOfficerList();
            return result;
        }

        [HttpPost("SubmitCheck")]
        public async Task<ReturnGenericStatus> SubmitCheck(ParamSaveChequeDetails value)
        {
            var result = await _unitOfWork.InwardClearingCheque.SubmitCheck(value);
            return result;
        }
    }
}
