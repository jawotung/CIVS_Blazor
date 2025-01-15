using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IBranchModelAuxServices
    {
        Task<PaginatedOutputServices<BranchModelAux>> GetBranchModelAuxList(string sKey = "", int page = 1);
        Task<ReturnGenericData<BranchModelAux>> GetBranchModelAux(int? id);
        Task<ReturnGenericDictionary> SaveBranchModelAux(BranchModelAux data);
        Task<ReturnGenericStatus> DeleteBranchModelAuxes(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintBranchModelAux(string sKey = "");
    }
}
