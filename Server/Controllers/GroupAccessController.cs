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
    public class GroupAccessController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupAccessController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetGroupAccessList")]
        public async Task<PaginatedOutput<GroupAccessModel>> GetGroupAccessList(int? page)
        {
            return await _unitOfWork.GroupAccess.GetGroupAccessList(page);
        }

        [HttpGet("GroupAccess")]
        public async Task<ReturnGenericData<GroupAccessModel>> GetGroupAccess(int id)
        {
            return await _unitOfWork.GroupAccess.GetGroupAccess(id);
        }

        [HttpGet("GetMenuList")]
        public async Task<ReturnGenericList<Application.Models.SelectListItem>> GetGroupAccessMenuList()
        {
            return await _unitOfWork.GroupAccess.GetGroupAccessMenuList();
        }

        [HttpGet("GroupAccessDetails")]
        public async Task<ReturnGenericData<GroupAccessModel>> GroupAccessDetails(int id)
        {
            return await _unitOfWork.GroupAccess.GroupAccessDetails(id);
        }

        [HttpPost("GroupAccessCreate")]
        public async Task<ReturnGenericStatus> Create(GroupAccessModel groupAccessModel)
        {
            return await _unitOfWork.GroupAccess.GroupAccessCreate(groupAccessModel);
        }

        [HttpPost("GroupAccessEdit")]
        public async Task<ReturnGenericStatus> GroupAccessEdit(GroupAccessModel groupAccessModel)
        {
            return await _unitOfWork.GroupAccess.GroupAccessEdit(groupAccessModel);
        }

        [HttpPost("GroupAccesDelete")]
        public async Task<ReturnGenericStatus> GroupAccessDeleteConfirmed(int id)
        {
            return await _unitOfWork.GroupAccess.GroupAccessDeleteConfirmed(id);
        }

        
    }
}
