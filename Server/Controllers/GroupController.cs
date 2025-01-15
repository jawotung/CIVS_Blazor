using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI;
using WebAPI.Handlers;

namespace BOCCIVS.server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetGroup")]
        public async Task<PaginatedOutput<GroupModel>> GetGroup(int? page)
        {
            return await _unitOfWork.Group.GetGroup(page);
        }

        [HttpGet("GetGroupDetails")]
        public async Task<ReturnGenericData<ReturnGroup>> GetGroupDetails(int id)
        {
            return await _unitOfWork.Group.GetGroupDetails(id);
        }

        [HttpPost("GroupCreate")]
        [Consumes("application/json")]
        public async Task<ReturnGenericStatus> GroupCreate(GroupModel groupModel)
        {
            return await _unitOfWork.Group.GroupCreate(groupModel);
        }

        [HttpPost("GroupEdit")]
        public async Task<ReturnGenericStatus> GroupEdit(int id, GroupModel groupModel)
        {
            return await _unitOfWork.Group.GroupEdit(id, groupModel);
        }

        [HttpPost("GroupDelete")]
        public async Task<ReturnGenericStatus> GroupDeleteConfirmed(int id)
        {
            return await _unitOfWork.Group.GroupDeleteConfirmed(id);
        }

        [HttpGet("GroupDownload")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintReport()
        {
            return await _unitOfWork.Group.PrintReport();
        }

        [HttpGet("GroupDownloadMatrix")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintMatrix()
        {
            return await _unitOfWork.Group.PrintMatrix();
        }

    }
}
