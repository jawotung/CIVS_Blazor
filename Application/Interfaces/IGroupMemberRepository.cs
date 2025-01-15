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
    public interface IGroupMemberRepositories
    {
        Task<ReturnGenericData<GroupMemberModel>> GetGroupMember(int id);
        Task<PaginatedOutput<GroupMemberModel>> GetGroupMemberList(int? page);
        Task<ReturnGenericList<SelectListItem>> GetUserTypeList();
        Task<ReturnGenericData<GroupMemberModel>> GroupMemberDetails(int id);
        Task<ReturnGenericStatus> GroupMemberCreate(GroupMemberModel groupMemberModel);
        Task<ReturnGenericStatus> GroupMemberEdit(GroupMemberModel groupMemberModel);
        Task<ReturnGenericStatus> GroupMemberDeleteConfirmed(int id);
    }
}
