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

namespace Application.Service
{
    public class InwardClearingChequeServices : IInwardClearingChequeServices
    {
        private readonly HttpClient httpClient;

        public InwardClearingChequeServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ReturnGenericDropdown> GetStatusList()
        {
            try
            {
                var response = await httpClient.GetAsync("/InwardClearingCheque/GetStatusList");
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericDropdown>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Task<ReturnGenericList<SelectListItem>> GetBranchList()
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnGenericData<PaginatedOutputServices<InwardClearingChequeDetailsModel>>> GetICCList(string key, int? page, ParamInwardClearingChequeGetList value)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "key", key ?? "" },
                { "page", page.ToString() },
                { "BSRTNFilter", value.BSRTNFilter ?? "" },
                { "DateFromFilter", value.DateFromFilter.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                { "DateToFilter", value.DateToFilter.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                { "AccountNumberFilter", value.AccountNumberFilter ?? "" },
                { "AmountFromFilter", value.AmountFromFilter ?? "" },
                { "AmountToFilter", value.AmountToFilter ?? "" },
                { "CheckImageFilter", value.CheckImageFilter ?? "" }
            };
            var queryString = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

            var response = await httpClient.GetAsync($"/InwardClearingCheque/GetICCList?{queryString}");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<PaginatedOutputServices<InwardClearingChequeDetailsModel>>>();
            return result;
           
        }

        public async Task<ReturnGenericData<CheckDetailModel>> GetICCDetails(int id)
        {
            var response = await httpClient.GetAsync($"/InwardClearingCheque/GetICCDetails?id={id}");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<CheckDetailModel>>();
            return result;
        }

        public async Task<ReturnGenericData<ReturnCheckImageDetailTransaction>> GetCheckImageDetails(string sKey)
        {
            var response = await httpClient.GetAsync($"/InwardClearingCheque/GetCheckImageDetails?sKey={sKey}");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnCheckImageDetailTransaction>>();
            return result;
        }

        public async Task<ReturnGenericData<ReturnViewSignatures>> GetViewSignatures(string accountNo)
        {
            var response = await httpClient.GetAsync($"/InwardClearingCheque/GetViewSignatures?accountNo={accountNo}");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnViewSignatures>>();
            return result;
        }

        public async Task<ReturnGenericList<SelectListItem>> GetUserAllowedAccess(string key, string amount)
        {
            var response = await httpClient.GetAsync($"/InwardClearingCheque/GetUserAllowedAccess?key={key}&amount={amount}");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericList<SelectListItem>>();
            return result;
        }

        public async Task<ReturnGenericList<SelectListItem>> GetReasonList()
        {
            var response = await httpClient.GetAsync($"/InwardClearingCheque/GetReasonList");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericList<SelectListItem>>();
            return result;
        }

        public async Task<ReturnGenericList<SelectListItem>> GetOfficerList()
        {
            var response = await httpClient.GetAsync($"/InwardClearingCheque/GetOfficerList");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericList<SelectListItem>>();
            return result;
        }

        public async Task<ReturnGenericStatus> SubmitCheck(ParamSaveChequeDetails value)
        {
            var response = await httpClient.PostAsJsonAsync($"/InwardClearingCheque/SubmitCheck", value);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
            return result;
        }
    }
}
