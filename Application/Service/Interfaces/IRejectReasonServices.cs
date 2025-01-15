using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IRejectReasonServices
    {
        Task<PaginatedOutputServices<RejectReasonModel>> GetRejectReasonList(string sKey = "", int page = 1);
        Task<ReturnGenericData<RejectReasonModel>> GetRejectReason(int? id);
        Task<ReturnGenericDictionary> SaveRejectReason(RejectReasonModel data);
        Task<ReturnGenericStatus> DeleteRejectReason(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintRejectReason(string sKey = "");
    }
}
