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
    public class MenuServices : IMenuServices
    {
        private readonly HttpClient httpClient;

        public MenuServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PaginatedOutputServices<MenuModel>> GetMenuList(string sKey = "", int page = 1)
        {
            try
            {
                var response = await httpClient.GetAsync("/Menu/GetMenuList?sKey=" + (sKey ?? "") + "&page=" + page);
                var result = await response.Content.ReadFromJsonAsync<PaginatedOutputServices<MenuModel>>();
                return result;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<MenuModel>> GetMenu(int? id)
        {
            var response = await httpClient.GetAsync("/Menu/GetMenu?id=" + id);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<MenuModel>>();
            return result;
        }
        public async Task<ReturnGenericDictionary> SaveMenu(MenuModel data)
        {
            ReturnGenericDictionary status = new ReturnGenericDictionary();
            var response = await httpClient.PostAsJsonAsync("/Menu/SaveMenu", data);
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
        public async Task<ReturnGenericStatus> DeleteMenu(int id)
        {
            try
            {
                string url = $"/Menu?id={id}";
                var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>(url, null);
                if (response.IsSuccessStatusCode)
                {
                    var status = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
                    return status;
                }
                else
                {
                    return new ReturnGenericStatus { StatusCode = "01", StatusMessage = "Failed to delete Menu." };
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintMenu(string sKey = "")
        {
            var response = await httpClient.GetAsync("/Menu/PrintMenu?sKey=" + sKey);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnDownloadPDF>>();
            return result;
        }
        public async Task<ReturnGenericDropdown> GetControllerList()
        {
            try
            {
                var response = await httpClient.GetAsync("/Menu/GetControllerList");
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericDropdown>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericDropdown> GetSubmenuList()
        {
            try
            {
                var response = await httpClient.GetAsync("/Menu/GetSubmenuList");
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericDropdown>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericDropdown> GetActionMethodList()
        {
            try
            {
                var response = await httpClient.GetAsync("/Menu/GetActionMethodList");
                var result = await response.Content.ReadFromJsonAsync<ReturnGenericDropdown>();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericDropdown> GetActionMethodParamList()
        {
            try
            {
                var response = await httpClient.GetAsync("/Menu/GetActionMethodParamList");
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
