using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAPI;

namespace BOCCIVS.server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class GroupMemberController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupMemberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetGroupMember")]
        public async Task<ReturnGenericData<GroupMemberModel>> GetGroupMember(int id)
        {
            return await _unitOfWork.GroupMember.GetGroupMember(id);
        }

        [HttpGet("GetUserTypeList")]
        public async Task<ReturnGenericList<Application.Models.SelectListItem>> GetUserTypeList()
        {
            return await _unitOfWork.GroupMember.GetUserTypeList();
        }

        [HttpPost("GroupMemberEdit")]
        public async Task<ReturnGenericStatus> GroupMemberEdit(GroupMemberModel groupMemberModel)
        {
            return await _unitOfWork.GroupMember.GroupMemberEdit(groupMemberModel);
        }

        [HttpGet("GroupMemberDetails")]
        public async Task<ReturnGenericData<GroupMemberModel>> GroupMemberDetails(int id)
        {
            return await _unitOfWork.GroupMember.GroupMemberDetails(id);
        }

        [HttpGet("GetGroupMemberList")]
        public async Task<PaginatedOutput<GroupMemberModel>> GetGroupMemberList(int? page)
        {
            return await _unitOfWork.GroupMember.GetGroupMemberList(page);
        }

        [HttpPost("GroupAccesDelete")]
        public async Task<ReturnGenericStatus> GroupMemberDeleteConfirmed(int id)
        {
            return await _unitOfWork.GroupMember.GroupMemberDeleteConfirmed(id);
        }
    }
}
