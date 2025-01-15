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
    public class AmountLimitsServices : IAmountLimitsServices
    {
        private readonly HttpClient httpClient;

        public AmountLimitsServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PaginatedOutputServices<AmountLimitsModel>> GetAmountLimitsList(string sKey = "", int page = 1)
        {
            try
            {
                var response = await httpClient.GetAsync("/AmountLimits/GetAmountLimitsList?page=" + page);
                var result = await response.Content.ReadFromJsonAsync<PaginatedOutputServices<AmountLimitsModel>>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<AmountLimitsModel>> GetAmountLimits(int? id)
        {
            var response = await httpClient.GetAsync("/AmountLimits/GetAmountLimits?id=" + id);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<AmountLimitsModel>>();
            return result;
        }
        public async Task<ReturnGenericDictionary> SaveAmountLimits(AmountLimitsModel data)
        {
            ReturnGenericDictionary status = new ReturnGenericDictionary();
            var response = await httpClient.PostAsJsonAsync("/AmountLimits/SaveAmountLimits", data);
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
        public async Task<ReturnGenericStatus> DeleteAmountLimits(int id)
        {
            try
            {
                string url = $"/AmountLimits?id={id}";
                var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>(url, null);
                if (response.IsSuccessStatusCode)
                {
                    var status = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
                    return status;
                }
                else
                {
                    return new ReturnGenericStatus { StatusCode = "01", StatusMessage = "Failed to delete Amount Limits." };
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintAmountLimits(string sKey = "")
        {
            var response = await httpClient.GetAsync("/AmountLimits/PrintAmountLimits?sKey=" + sKey);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnDownloadPDF>>();
            return result;
        }
        public async Task<ReturnGenericDropdown> GetAllowedAction()
        {
            try
            {
                var response = await httpClient.GetAsync("/AmountLimits/GetAllowedAction");
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericDropdown>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}