using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IRejectReasonRepository
    {
        Task<PaginatedOutput<RejectReasonModel>> GetRejectReasonList(int page = 1);
        Task<ReturnGenericData<RejectReasonModel>> GetRejectReason(int? id);
        Task<ReturnGenericStatus> SaveRejectReason(RejectReasonModel data);
        Task<ReturnGenericStatus> DeleteRejectReason(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintRejectReason();
    }
}
