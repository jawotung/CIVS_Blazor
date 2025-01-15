using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebAPI;

namespace BOCCIVS.server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserTypeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetUserTypeList")]
        public async Task<PaginatedOutput<UserTypeModel>> GetUserTypeList(string sKey = "", int page = 1)
        {
            try
            {
                PaginatedOutput<UserTypeModel> Paginate = await _unitOfWork.UserType.GetUserTypeList(sKey, page);

                return Paginate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("GetUserType")]
        public async Task<ReturnGenericData<UserTypeModel>> GetUserType(int? id)
        {
            ReturnGenericData<UserTypeModel> r = await _unitOfWork.UserType.GetUserType(id);
            return r;
        }
        [HttpPost("SaveUserType")]
        public async Task<ReturnGenericStatus> SaveUserType([FromBody] UserTypeModel data)
        {
            ReturnGenericStatus status = new();
            if (ModelState.IsValid)
            {
                status = await _unitOfWork.UserType.SaveUserType(data);
            }
            return status;
        }
        // POST: UserType/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ReturnGenericStatus> DeleteConfirmed(int id)
        {
            ReturnGenericStatus status = new();
            status = await _unitOfWork.UserType.DeleteUserType(id);
            return status;
        }
        [HttpGet("PrintUserType")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintUserType()
        {
            return await _unitOfWork.UserType.PrintUserType();
        }
    }
}
