using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using WebAPI;

namespace BOCCIVS.server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class BranchModelAuxesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public BranchModelAuxesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetBranchModelAuxList")]
        public async Task<PaginatedOutput<BranchModelAux>> GetBranchModelAuxList(string sKey = "", int page = 1)
        {
            try
            {
                PaginatedOutput<BranchModelAux> Paginate = await _unitOfWork.BranchModelAux.GetBranchModelAuxList(sKey, page);

                return Paginate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("GetBranchModelAux")]
        public async Task<ReturnGenericData<BranchModelAux>> GetBranchModelAux(int? id)
        {
            ReturnGenericData<BranchModelAux> r = await _unitOfWork.BranchModelAux.GetBranchModelAux(id);
            return r;
        }
        [HttpPost("SaveBranchModelAux")]
        public async Task<ReturnGenericStatus> SaveBranchModelAux([FromBody] BranchModelAux data)
        {
            ReturnGenericStatus status = new();
            if (ModelState.IsValid)
            {
                status = await _unitOfWork.BranchModelAux.SaveBranchModelAux(data);
            }
            return status;
        }
        // POST: Branch/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ReturnGenericStatus> DeleteConfirmed(int id)
        {
            ReturnGenericStatus status = new();
            status = await _unitOfWork.BranchModelAux.DeleteBranchModelAux(id);
            return status;
        }
        [HttpGet("PrintBranchModelAux")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintBranchModelAux(string sKey = "")
        {
            return await _unitOfWork.BranchModelAux.PrintBranchModelAux(sKey);
        }
    }
}
