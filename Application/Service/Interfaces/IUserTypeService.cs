using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IUserTypeServices
    {
        Task<PaginatedOutputServices<UserTypeModel>> GetUserTypeList(string sKey = "", int page = 1);
        Task<ReturnGenericData<UserTypeModel>> GetUserType(int? id);
        Task<ReturnGenericDictionary> SaveUserType(UserTypeModel data);
        Task<ReturnGenericStatus> DeleteUserType(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintUserType(string sKey = "");
    }
}
