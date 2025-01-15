using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IUserAmountLimitServices
    {
        Task<PaginatedOutputServices<UserAmountLimitModel>> GetUserAmountLimitList(string sKey = "", int page = 1);
        Task<ReturnGenericData<UserAmountLimitModel>> GetUserAmountLimit(int? id);
        Task<ReturnGenericDictionary> SaveUserAmountLimit(UserAmountLimitModel data);
        Task<ReturnGenericStatus> DeleteUserAmountLimit(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintUserAmountLimit(string sKey = "");
        Task<ReturnGenericDropdown> GetUserTypeList();
        Task<ReturnGenericDropdown> GetUserList(string type);
        Task<ReturnGenericDropdown> GetAmountLimitList();
    }
}
