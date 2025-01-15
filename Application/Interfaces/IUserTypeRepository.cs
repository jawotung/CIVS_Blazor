using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IUserTypeRepository
    {
        Task<PaginatedOutput<UserTypeModel>> GetUserTypeList(string Key = "",int page = 1);
        Task<ReturnGenericData<UserTypeModel>> GetUserType(int? id);
        Task<ReturnGenericStatus> SaveUserType(UserTypeModel data);
        Task<ReturnGenericStatus> DeleteUserType(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintUserType();
    }
}
