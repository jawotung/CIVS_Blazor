using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.Drawing;
using Application.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class InwardClearingChequeReportController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public InwardClearingChequeReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("GetReportList")]
        //[ValidateAntiForgeryToken]
        public async Task<PaginatedOutput<InwardClearingReportModel>> GetReportList(string Key, int page = 1, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null)
        {
            try
            {
                PaginatedOutput<InwardClearingReportModel> Paginate = await _unitOfWork.InwardClearingChequeReport.GetReportList(Key, page, dateFromFilter, dateToFilter, BSRTNFilter);
                return Paginate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpGet("GetBuddyBranches")]
        //[ValidateAntiForgeryToken]
        public async Task<ReturnGenericDropdown> GetBuddyBranches()
        {
            return await _unitOfWork.InwardClearingChequeReport.GetBuddyBranches();
        }
        [HttpGet("GetMCDetails")]
        //[ValidateAntiForgeryToken]
        public async Task<ReturnGenericData<MCCheckDetailModel>> GetMCDetails(string acctAmt)
        {
            return await _unitOfWork.InwardClearingChequeReport.GetMCDetails(acctAmt);
        }

        [HttpGet("PrintReport")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintReport(string Key, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null)
        {
            return await _unitOfWork.InwardClearingChequeReport.PrintReport(Key, dateFromFilter, dateToFilter, BSRTNFilter);
        }
        [HttpGet("PrintReportWithImages")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintReportWithImages(string Key = "", string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null)
        {
            return await _unitOfWork.InwardClearingChequeReport.PrintReportWithImages(Key, dateFromFilter, dateToFilter, BSRTNFilter);
        }
    }
}
