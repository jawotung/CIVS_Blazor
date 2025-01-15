using Application.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IInwardClearingChequeUploadRepository
    {
        Task<ReturnGenericData<InwardClearingChequeUploadReturn>> UploadImage(MCheckImageUpload data);
        Task<ReturnGenericData<MReturnSaveImage>> SaveImage(InwardClearingChequeUploadReturn data);
    }
}
