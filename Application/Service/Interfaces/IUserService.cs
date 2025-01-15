using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IUserServices
    {
        Task<PaginatedOutputServices<UserModel>> GetUserList(string sKey = "", string key = "", int page = 1);
        Task<ReturnGenericData<UserModel>> GetUser(int? id);
        Task<ReturnGenericDictionary> SaveUser(UserModel data);
        Task<ReturnGenericStatus> DeleteUser(int id);
        Task<ReturnGenericStatus> Able(int id, bool mode);
        Task<ReturnGenericStatus> Activate(int id, bool mode);
        Task<ReturnGenericDropdown> GetUserTypeList();
        Task<ReturnGenericDropdown> GetBranchOfAssignmentList();
        Task<ReturnGenericDropdown> GetGroupList();
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintUser(string sKey = "");
    }
}
