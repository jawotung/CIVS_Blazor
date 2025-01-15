using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebAPI;

namespace BOCCIVS.server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserAmountLimitController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public UserAmountLimitController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetUserAmountLimitList")]
        public async Task<PaginatedOutput<UserAmountLimitModel>> GetUserAmountLimitList(int page = 1)
        {
            try
            {
                PaginatedOutput<UserAmountLimitModel> Paginate = await _unitOfWork.UserAmountLimit.GetUserAmountLimitList(page);

                return Paginate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("GetUserAmountLimit")]
        public async Task<ReturnGenericData<UserAmountLimitModel>> GetUserAmountLimit(int? id)
        {
            ReturnGenericData<UserAmountLimitModel> r = await _unitOfWork.UserAmountLimit.GetUserAmountLimit(id);
            return r;
        }
        [HttpPost("SaveUserAmountLimit")]
        public async Task<ReturnGenericStatus> SaveUserAmountLimit([FromBody] UserAmountLimitModel data)
        {
            ReturnGenericStatus status = new();
            if (ModelState.IsValid)
            {
                status = await _unitOfWork.UserAmountLimit.SaveUserAmountLimit(data);
            }
            return status;
        }
        // POST: UserAmountLimit/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ReturnGenericStatus> DeleteConfirmed(int id)
        {
            ReturnGenericStatus status = new();
            status = await _unitOfWork.UserAmountLimit.DeleteUserAmountLimit(id);
            return status;
        }
        [HttpGet("PrintUserAmountLimit")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintUserAmountLimit()
        {
            return await _unitOfWork.UserAmountLimit.PrintUserAmountLimit();
        }
        [HttpGet("GetUserTypeList")]
        public async Task<ReturnGenericDropdown> GetUserTypeList()
        {
            return await _unitOfWork.UserAmountLimit.GetUserTypeList();
        }
        [HttpGet("GetUserList")]
        public async Task<ReturnGenericDropdown> GetUserList(string type = "")
        {
            return await _unitOfWork.UserAmountLimit.GetUserList(type);
        }
        [HttpGet("GetAmountLimitList")]
        public async Task<ReturnGenericDropdown> GetAmountLimitList()
        {
            return await _unitOfWork.UserAmountLimit.GetAmountLimitList();
        }
    }
}
