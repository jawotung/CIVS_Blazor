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
    public class EmailTemplateController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmailTemplateController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetEmailTemplateList")]
        public async Task<PaginatedOutput<EmailTemplateModel>> GetEmailTemplateList(int page = 1)
        {
            try
            {
                PaginatedOutput<EmailTemplateModel> Paginate = await _unitOfWork.EmailTemplate.GetEmailTemplateList(page);

                return Paginate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet("GetEmailTemplate")]
        public async Task<ReturnGenericData<EmailTemplateModel>> GetEmailTemplate(int? id)
        {
            ReturnGenericData<EmailTemplateModel> r = await _unitOfWork.EmailTemplate.GetEmailTemplate(id);
            return r;
        }
        [HttpPost("SaveEmailTemplate")]
        public async Task<ReturnGenericStatus> SaveEmailTemplate([FromBody] EmailTemplateModel data)
        {
            ReturnGenericStatus status = new();
            if (ModelState.IsValid)
            {
                status = await _unitOfWork.EmailTemplate.SaveEmailTemplate(data);
            }
            return status;
        }
        // POST: EmailTemplate/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ReturnGenericStatus> DeleteEmailTemplate(int id)
        {
            ReturnGenericStatus status = new();
            status = await _unitOfWork.EmailTemplate.DeleteEmailTemplate(id);
            return status;
        }
    }
}
