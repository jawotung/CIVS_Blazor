using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Imaging;
using System.Drawing;
using Application.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class InwardClearingChequeUploadController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public InwardClearingChequeUploadController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("UploadCheque")]
        [Consumes("application/json")]
        //[ValidateAntiForgeryToken]
        public async Task<ReturnGenericData<InwardClearingChequeUploadReturn>> UploadCheque(MCheckImageUpload data)
        {
            ReturnGenericData<InwardClearingChequeUploadReturn> status = new();
            if (ModelState.IsValid)
            {
                ClientFormFile x = data.FileUpload;
                using (var memorystream = new MemoryStream(x.FileByte))
                {
                    x.file = new FormFile(memorystream, 0, x.FileByte.Length, x.Name, x.FileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = x.ContentType
                    };
                }
                status = await _unitOfWork.InwardClearingChequeUpload.UploadImage(data);
            }
            return status;
        }
        [HttpPost("SaveImage")]
        //[ValidateAntiForgeryToken]
        public async Task<ReturnGenericData<MReturnSaveImage>> SaveImage(InwardClearingChequeUploadReturn data)
        {
            return await _unitOfWork.InwardClearingChequeUpload.SaveImage(data);
        }
    }
}
