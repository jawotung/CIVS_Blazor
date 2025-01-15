using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IUserAmountLimitRepository
    {
        Task<PaginatedOutput<UserAmountLimitModel>> GetUserAmountLimitList(int page = 1);
        Task<ReturnGenericData<UserAmountLimitModel>> GetUserAmountLimit(int? id);
        Task<ReturnGenericStatus> SaveUserAmountLimit(UserAmountLimitModel data);
        Task<ReturnGenericStatus> DeleteUserAmountLimit(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintUserAmountLimit();
        Task<ReturnGenericDropdown> GetUserTypeList();
        Task<ReturnGenericDropdown> GetUserList(string type);
        Task<ReturnGenericDropdown> GetAmountLimitList();
    }
}
