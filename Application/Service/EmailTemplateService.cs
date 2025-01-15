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
    public class EmailTemplateServices : IEmailTemplateServices
    {
        private readonly HttpClient httpClient;

        public EmailTemplateServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PaginatedOutputServices<EmailTemplateModel>> GetEmailTemplateList(string sKey = "", int page = 1)
        {
            try
            {
                var response = await httpClient.GetAsync("/EmailTemplate/GetEmailTemplateList?page=" + page);
                var result = await response.Content.ReadFromJsonAsync<PaginatedOutputServices<EmailTemplateModel>>();
                return result;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<EmailTemplateModel>> GetEmailTemplate(int? id)
        {
            var response = await httpClient.GetAsync("/EmailTemplate/GetEmailTemplate?id=" + id);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<EmailTemplateModel>>();
            return result;
        }
        public async Task<ReturnGenericDictionary> SaveEmailTemplate(EmailTemplateModel data)
        {
            ReturnGenericDictionary status = new ReturnGenericDictionary();
            var response = await httpClient.PostAsJsonAsync("/EmailTemplate/SaveEmailTemplate", data);
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
        public async Task<ReturnGenericStatus> DeleteEmailTemplate(int id)
        {
            try
            {
                string url = $"/EmailTemplate?id={id}";
                var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>(url, null);
                if (response.IsSuccessStatusCode)
                {
                    var status = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
                    return status;
                }
                else
                {
                    return new ReturnGenericStatus { StatusCode = "01", StatusMessage = "Failed to delete EmailTemplate." };
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintEmailTemplate(string sKey = "")
        {
            var response = await httpClient.GetAsync("/EmailTemplate/PrintEmailTemplate?sKey=" + sKey);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnDownloadPDF>>();
            return result;
        }
    }
}
