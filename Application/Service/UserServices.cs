using Application.Interfaces;
using Application.Models;
using Application.Service.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebAPI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Service
{
    public class UserServices : IUserServices
    {
        private readonly HttpClient httpClient;

        public UserServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PaginatedOutputServices<UserModel>> GetUserList(string sKey = "", string key = "", int page = 1)
        {
            try
            {
                var response = await httpClient.GetAsync("/User/GetUserList?sKey=" + (sKey ?? "") + "&key=" + (key ?? "") + "&page=" + page);
                var result = await response.Content.ReadFromJsonAsync<PaginatedOutputServices<UserModel>>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<UserModel>> GetUser(int? id)
        {
            var response = await httpClient.GetAsync("/User/GetUser?id=" + id);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<UserModel>>();
            return result;
        }
        public async Task<ReturnGenericDictionary> SaveUser(UserModel data)
        {
            ReturnGenericDictionary status = new ReturnGenericDictionary();
            var response = await httpClient.PostAsJsonAsync("/User/SaveUser", data);
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
        public async Task<ReturnGenericStatus> DeleteUser(int id)
        {
            try
            {
                string url = $"/User?id={id}";
                var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>(url, null);
                if (response.IsSuccessStatusCode)
                {
                    var status = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
                    return status;
                }
                else
                {
                    return new ReturnGenericStatus { StatusCode = "01", StatusMessage = "Failed to delete User." };
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericStatus> Able(int id, bool mode)
        {
            try
            {
                string url = $"/User/Able?id={id}&mode={mode}";
                var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>(url, null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
                    return result;
                }
                else
                {
                    // Handle unsuccessful response here
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericStatus> Activate(int id, bool mode)
        {
            try
            {
                string url = $"/User/Activate?id={id}&mode={mode}";
                var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>(url, null);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
                    return result;
                }
                else
                {
                    // Handle unsuccessful response here
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericDropdown> GetBranchOfAssignmentList()
        {
            try
            {
                var response = await httpClient.GetAsync("/User/GetBranchOfAssignmentList");
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericDropdown>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericDropdown> GetUserTypeList()
        {
            try
            {
                var response = await httpClient.GetAsync("/User/GetUserTypeList");
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericDropdown>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericDropdown> GetGroupList()
        {
            try
            {
                var response = await httpClient.GetAsync("/User/GetGroupList");
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericDropdown>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintUser(string sKey = "")
        {
            var response = await httpClient.GetAsync("/User/PrintUser?sKey=" + sKey);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnDownloadPDF>>();
            return result;
        }
    }
}
