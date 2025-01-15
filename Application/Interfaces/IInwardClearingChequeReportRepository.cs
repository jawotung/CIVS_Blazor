using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IInwardClearingChequeReportRepository
    {
        Task<PaginatedOutput<InwardClearingReportModel>> GetReportList(string Key, int page = 1, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null);
        Task<ReturnGenericDropdown> GetBuddyBranches();
        Task<ReturnGenericData<MCCheckDetailModel>> GetMCDetails(string acctAmt);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintReport(string Key, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintReportWithImages(string Key, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null);
    }
}
