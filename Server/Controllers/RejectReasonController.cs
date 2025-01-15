using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class RejectReasonController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public RejectReasonController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetRejectReasonList")]
        public async Task<PaginatedOutput<RejectReasonModel>> GetRejectReasonList(int page = 1)
        {
            try
            {
                PaginatedOutput<RejectReasonModel> Paginate = await _unitOfWork.RejectReason.GetRejectReasonList(page);

                return Paginate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("GetRejectReason")]
        public async Task<ReturnGenericData<RejectReasonModel>> GetRejectReason(int? id)
        {
            ReturnGenericData<RejectReasonModel> r = await _unitOfWork.RejectReason.GetRejectReason(id);
            return r;
        }
        [HttpPost("SaveRejectReason")]
        public async Task<ReturnGenericStatus> SaveRejectReason([FromBody] RejectReasonModel data)
        {
            ReturnGenericStatus status = new();
            if (ModelState.IsValid)
            {
                status = await _unitOfWork.RejectReason.SaveRejectReason(data);
            }
            return status;
        }
        // POST: RejectReason/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ReturnGenericStatus> DeleteConfirmed(int id)
        {
            ReturnGenericStatus status = new();
            status = await _unitOfWork.RejectReason.DeleteRejectReason(id);
            return status;
        }
        [HttpGet("PrintRejectReason")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintRejectReason()
        {
            return await _unitOfWork.RejectReason.PrintRejectReason();
        }
    }
}
