using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebAPI;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetUserList")]
        public async Task<PaginatedOutput<UserModel>> GetUserList(string sKey = "", string key = "", int page = 1)
        {
            try
            {
                PaginatedOutput<UserModel> Paginate = await _unitOfWork.User.GetUserList(sKey, key, page);

                return Paginate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("GetUser")]
        public async Task<ReturnGenericData<UserModel>> GetUser(int? id)
        {
            ReturnGenericData<UserModel> r = await _unitOfWork.User.GetUser(id);
            return r;
        }
        [HttpPost("SaveUser")]
        public async Task<ReturnGenericStatus> SaveUser([FromBody] UserModel data)
        {
            ReturnGenericStatus status = new();
            if (ModelState.IsValid)
            {
                status = await _unitOfWork.User.SaveUser(data);
            }
            return status;
        }
        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ReturnGenericStatus> DeleteUser(int id)
        {
            ReturnGenericStatus status = new();
            status = await _unitOfWork.User.DeleteUser(id);
            return status;
        }
        [HttpPost("Able")]
        public async Task<ReturnGenericStatus> Able(int id, bool mode)
        {
            return await _unitOfWork.User.Able(id, mode);
        }
        [HttpPost("Activate")]
        public async Task<ReturnGenericStatus> Activate(int id, bool mode)
        {
            return await _unitOfWork.User.Activate(id, mode);
        }
        [HttpGet("GetUserTypeList")]
        public async Task<ReturnGenericDropdown> GetUserTypeList()
        {
            return await _unitOfWork.User.GetUserTypeList();
        }
        [HttpGet("GetBranchOfAssignmentList")]
        public async Task<ReturnGenericDropdown> GetBranchOfAssignmentList()
        {
            return await _unitOfWork.User.GetBranchOfAssignmentList();
        }
        [HttpGet("GetGroupList")]
        public async Task<ReturnGenericDropdown> GetGroupList()
        {
            return await _unitOfWork.User.GetGroupList();
        }
        [HttpGet("PrintUser")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintUser(string key = "")
        {
            return await _unitOfWork.User.PrintUser(key);
        }
    }
}
