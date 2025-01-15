using Application.Interfaces;
using Application.Models;
using Application.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Service
{
    public class RejectReasonServices : IRejectReasonServices
    {
        private readonly HttpClient httpClient;

        public RejectReasonServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PaginatedOutputServices<RejectReasonModel>> GetRejectReasonList(string sKey = "", int page = 1)
        {
            try
            {
                var response = await httpClient.GetAsync("/RejectReason/GetRejectReasonList?sKey=" + (sKey ?? "") + "&page=" + page);
                var result = await response.Content.ReadFromJsonAsync<PaginatedOutputServices<RejectReasonModel>>();
                return result;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<RejectReasonModel>> GetRejectReason(int? id)
        {
            var response = await httpClient.GetAsync("/RejectReason/GetRejectReason?id=" + id);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<RejectReasonModel>>();
            return result;
        }
        public async Task<ReturnGenericDictionary> SaveRejectReason(RejectReasonModel data)
        {
            ReturnGenericDictionary status = new ReturnGenericDictionary();
            var response = await httpClient.PostAsJsonAsync("/RejectReason/SaveRejectReason", data);
            if (response.IsSuccessStatusCode)
            {
                ReturnGenericStatus r = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
                status.StatusCode = r.StatusCode;
                status.StatusMessage = r.StatusMessage;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var Error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                status.StatusCode = "01";
                status.StatusMessage = Error.Title;
                status.Data = Error.Errors;
            }
            else
            {
                status.StatusCode = "01";
                status.StatusMessage = response.StatusCode.ToString();
            }
            return status;
        }
        public async Task<ReturnGenericStatus> DeleteRejectReason(int id)
        {
            try
            {
                string url = $"/RejectReason?id={id}";
                var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>(url, null);
                if (response.IsSuccessStatusCode)
                {
                    var status = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
                    return status;
                }
                else
                {
                    return new ReturnGenericStatus { StatusCode = "01", StatusMessage = "Failed to delete RejectReason." };
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintRejectReason(string sKey = "")
        {
            var response = await httpClient.GetAsync("/RejectReason/PrintRejectReason?sKey=" + sKey);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnDownloadPDF>>();
            return result;
        }
    }
}
