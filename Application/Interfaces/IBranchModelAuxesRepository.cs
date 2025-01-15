using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IBranchModelAuxesRepository
    {
        Task<PaginatedOutput<BranchModelAux>> GetBranchModelAuxList(string sKey = "", int page = 1);
        Task<ReturnGenericData<BranchModelAux>?> GetBranchModelAux(int? id);
        Task<ReturnGenericStatus> SaveBranchModelAux(BranchModelAux data);
        Task<ReturnGenericStatus> DeleteBranchModelAux(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintBranchModelAux(string sKey = "");
    }
}
