using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IGroupAccessServices
    {
        Task<ReturnGenericData<GroupAccessModel>> GetGroupAccess(int id);
        Task<PaginatedOutputServices<GroupAccessModel>> GetGroupAccessList(int? page);
        Task<ReturnGenericList<SelectListItem>> GetGroupAccessMenuList();
        Task<ReturnGenericStatus> GroupAccessEdit(GroupAccessModel groupAccessModel);
        Task<ReturnGenericData<GroupAccessModel>> GroupAccessDetails(int id);
        Task<ReturnGenericStatus> GroupAccessCreate(GroupAccessModel groupAccessModel);
        Task<ReturnGenericStatus> GroupAccessDeleteConfirmed(int id);
    }
}
