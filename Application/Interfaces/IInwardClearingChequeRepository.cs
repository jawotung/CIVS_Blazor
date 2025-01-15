using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IInwardClearingChequeRepository
    {
        ReturnGenericDropdown GetStatusList();
        Task<ReturnGenericData<PaginatedOutput<InwardClearingChequeDetailsModel>>> GetICCList(string? key, int? page, ParamInwardClearingChequeGetList value);
        Task<ReturnGenericList<SelectListItem>> GetBranchList();
        Task<ReturnGenericData<CheckDetailModel>> GetICCDetails(int id);
        Task<ReturnGenericData<ReturnCheckImageDetailTransaction>> GetCheckImageDetails(string sKey);
        Task<ReturnGenericData<ReturnViewSignatures>> GetViewSignatures(string accountNo);
        Task<ReturnGenericList<SelectListItem>> GetUserAllowedAccess(string? key, string amount);
        Task<ReturnGenericList<SelectListItem>> GetReasonList();
        Task<ReturnGenericList<SelectListItem>> GetOfficerList(); 
        Task<ReturnGenericStatus> SubmitCheck(ParamSaveChequeDetails value);

    }
}
