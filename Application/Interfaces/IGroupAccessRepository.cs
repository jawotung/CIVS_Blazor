using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IGroupAccessRepositories
    {
        Task<ReturnGenericData<GroupAccessModel>> GetGroupAccess(int id);
        Task<PaginatedOutput<GroupAccessModel>> GetGroupAccessList(int? page);
        Task<ReturnGenericList<SelectListItem>> GetGroupAccessMenuList();
        Task<ReturnGenericData<GroupAccessModel>> GroupAccessDetails(int id);
        Task<ReturnGenericStatus> GroupAccessCreate(GroupAccessModel groupAccessModel);
        Task<ReturnGenericStatus> GroupAccessEdit(GroupAccessModel groupAccessModel);
        Task<ReturnGenericStatus> GroupAccessDeleteConfirmed(int id);
    }
}
