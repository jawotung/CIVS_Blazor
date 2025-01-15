using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using WebAPI;

namespace BOCCIVS.server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AmountLimitsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public AmountLimitsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetAmountLimitsList")]
        public async Task<PaginatedOutput<AmountLimitsModel>> GetAmountLimitsList(int page = 1)
        {
            try
            {
                PaginatedOutput<AmountLimitsModel> Paginate = await _unitOfWork.AmountLimits.GetAmountLimitsList(page);

                return Paginate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("GetAmountLimits")]
        public async Task<ReturnGenericData<AmountLimitsModel>> GetAmountLimits(int? id)
        {
            ReturnGenericData<AmountLimitsModel> r = await _unitOfWork.AmountLimits.GetAmountLimits(id);
            return r;
        }
        [HttpPost("SaveAmountLimits")]
        public async Task<ReturnGenericStatus> SaveAmountLimits([FromBody] AmountLimitsModel data)
        {
            ReturnGenericStatus status = new();
            if (ModelState.IsValid)
            {
                status = await _unitOfWork.AmountLimits.SaveAmountLimits(data);
            }
            return status;
        }
        // POST: AmountLimits/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ReturnGenericStatus> DeleteAmountLimits(int id)
        {
            ReturnGenericStatus status = new();
            status = await _unitOfWork.AmountLimits.DeleteAmountLimits(id);
            return status;
        }
        [HttpGet("PrintAmountLimits")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintAmountLimits()
        {
            return await _unitOfWork.AmountLimits.PrintAmountLimits();
        }
        [HttpGet("GetAllowedAction")]
        public async Task<ReturnGenericDropdown> GetAllowedAction()
        {
            return await _unitOfWork.AmountLimits.GetAllowedAction();
        }
    }
}
