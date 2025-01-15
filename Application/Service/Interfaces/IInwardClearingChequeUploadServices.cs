using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IInwardClearingChequeUploadServices
    {
        Task<ReturnGenericData<InwardClearingChequeUploadReturn>> UploadCheque(MCheckImageUpload data);
        Task<ReturnGenericData<MReturnSaveImage>> SaveImage(InwardClearingChequeUploadReturn data);
    }
}
