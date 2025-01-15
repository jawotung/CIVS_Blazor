using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IBranchRepository
    {
        Task<PaginatedOutput<BranchModel>> GetBranchList(string sKey = "", int page = 1);
        Task<ReturnGenericData<BranchModel>> GetBranch(int? id);
        Task<ReturnGenericStatus> SaveBranch(BranchModel data);
        Task<ReturnGenericStatus> DeleteBranch(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintBranch(string sKey = "");
    }
}
