using Application.Interfaces;
using Application.Models;
using Application.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Application.Service
{
    public class InwardClearingChequeUploadServices : IInwardClearingChequeUploadServices
    {
        private readonly HttpClient httpClient;

        public InwardClearingChequeUploadServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ReturnGenericData<InwardClearingChequeUploadReturn>> UploadCheque(MCheckImageUpload data)
        {
            ReturnGenericData<InwardClearingChequeUploadReturn> status = new ReturnGenericData<InwardClearingChequeUploadReturn>();
            var response = await httpClient.PostAsJsonAsync("/InwardClearingChequeUpload/UploadCheque", data);
            if (response.IsSuccessStatusCode)
            {
                ReturnGenericData<InwardClearingChequeUploadReturn> r = await response.Content.ReadFromJsonAsync<ReturnGenericData<InwardClearingChequeUploadReturn>>();
                status.StatusCode = r.StatusCode;
                status.StatusMessage = r.StatusMessage;
                status.Data = r.Data;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var Error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                status.StatusCode = "01";
                status.StatusMessage = Error.Title;
            }
            else
            {
                status.StatusCode = "01";
                status.StatusMessage = response.StatusCode.ToString();
            }
            return status;
        }
        public async Task<ReturnGenericData<MReturnSaveImage>> SaveImage(InwardClearingChequeUploadReturn data)
        {
            ReturnGenericData<MReturnSaveImage> status = new ReturnGenericData<MReturnSaveImage>();
            var response = await httpClient.PostAsJsonAsync("/InwardClearingChequeUpload/SaveImage", data);
            if (response.IsSuccessStatusCode)
            {
                ReturnGenericData<MReturnSaveImage> r = await response.Content.ReadFromJsonAsync<ReturnGenericData<MReturnSaveImage>>();
                status.StatusCode = r.StatusCode;
                status.StatusMessage = r.StatusMessage;
                status.Data = r.Data;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var Error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                status.StatusCode = "01";
                status.StatusMessage = Error.Title;
            }
            else
            {
                status.StatusCode = "01";
                status.StatusMessage = response.StatusCode.ToString();
            }
            return status;
        }

    }
}
