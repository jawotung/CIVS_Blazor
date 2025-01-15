using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IGroupMemberServices
    {
        Task<ReturnGenericData<GroupMemberModel>> GetGroupMember(int id);
        Task<PaginatedOutputServices<GroupMemberModel>> GetGroupMemberList(int? page);
        Task<ReturnGenericList<SelectListItem>> GetUserTypeList();
        Task<ReturnGenericData<GroupMemberModel>> GroupMemberDetails(int id);
        Task<ReturnGenericStatus> GroupMemberCreate(GroupMemberModel groupMemberModel);
        Task<ReturnGenericStatus> GroupMemberEdit(GroupMemberModel groupMemberModel);
        Task<ReturnGenericStatus> GroupMemberDeleteConfirmed(int id);
    }
}
