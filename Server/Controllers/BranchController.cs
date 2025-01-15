using Application.Interfaces;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Security;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DeviceDetectorNET;
using WebAPI;
using Application.Models;
using YamlDotNet.Core.Tokens;
using Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;

namespace BOCCIVS.server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class BranchController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public BranchController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetBranchList")]
        public async Task<PaginatedOutput<BranchModel>> GetBranchList(string sKey = "", int page = 1)
        {
            try
            {
                PaginatedOutput<BranchModel> Paginate = await _unitOfWork.Branch.GetBranchList(sKey, page);

                return Paginate;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        [HttpGet("GetBranch")]
        public async Task<ReturnGenericData<BranchModel>> GetBranch(int? id)
        {
            ReturnGenericData<BranchModel> r = await _unitOfWork.Branch.GetBranch(id);
            return r;
        }
        [HttpPost("SaveBranch")]
        public async Task<ReturnGenericStatus> SaveBranch(BranchModel data)
        {
            ReturnGenericStatus status = new();
            if (ModelState.IsValid)
            {
                status = await _unitOfWork.Branch.SaveBranch(data);
            }
            return status;
        }
        // POST: Branch/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ReturnGenericStatus> DeleteBranch(int id)
        {
            ReturnGenericStatus status = new();
            status = await _unitOfWork.Branch.DeleteBranch(id);
            return status;
        }
        [HttpGet("PrintBranch")]
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintBranch(string sKey = "")
        {
            return await _unitOfWork.Branch.PrintBranch(sKey);
        }
    }
}
