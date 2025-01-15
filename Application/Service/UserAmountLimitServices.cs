
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
    public class UserAmountLimitServices : IUserAmountLimitServices
    {
        private readonly HttpClient httpClient;

        public UserAmountLimitServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PaginatedOutputServices<UserAmountLimitModel>> GetUserAmountLimitList(string sKey = "", int page = 1)
        {
            try
            {
                var response = await httpClient.GetAsync("/UserAmountLimit/GetUserAmountLimitList?sKey=" + (sKey ?? "") + "&page=" + page);
                var result = await response.Content.ReadFromJsonAsync<PaginatedOutputServices<UserAmountLimitModel>>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<UserAmountLimitModel>> GetUserAmountLimit(int? id)
        {
            var response = await httpClient.GetAsync("/UserAmountLimit/GetUserAmountLimit?id=" + id);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<UserAmountLimitModel>>();
            return result;
        }
        public async Task<ReturnGenericDictionary> SaveUserAmountLimit(UserAmountLimitModel data)
        {
            ReturnGenericDictionary status = new ReturnGenericDictionary();
            var response = await httpClient.PostAsJsonAsync("/UserAmountLimit/SaveUserAmountLimit", data);
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
        public async Task<ReturnGenericStatus> DeleteUserAmountLimit(int id)
        {
            try
            {
                string url = $"/UserAmountLimit?id={id}";
                var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>(url, null);
                if (response.IsSuccessStatusCode)
                {
                    var status = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
                    return status;
                }
                else
                {
                    return new ReturnGenericStatus { StatusCode = "01", StatusMessage = "Failed to delete UserAmountLimit." };
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintUserAmountLimit(string sKey = "")
        {
            var response = await httpClient.GetAsync("/UserAmountLimit/PrintUserAmountLimit?sKey=" + sKey);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnDownloadPDF>>();
            return result;
        }
        public async Task<ReturnGenericDropdown> GetUserTypeList()
        {
            try
            {
                var response = await httpClient.GetAsync("/UserAmountLimit/GetUserTypeList");
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericDropdown>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericDropdown> GetUserList(string type)
        {
            try
            {
                var response = await httpClient.GetAsync("/UserAmountLimit/GetUserList?type=" + type);
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericDropdown>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericDropdown> GetAmountLimitList()
        {
            try
            {
                var response = await httpClient.GetAsync("/UserAmountLimit/GetAmountLimitList");
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
