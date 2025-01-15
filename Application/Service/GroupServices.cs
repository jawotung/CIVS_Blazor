using Application.Interfaces;
using Application.Models;
using Application.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service
{
    public class GroupServices : IGroupServices
    {
        private readonly HttpClient httpClient;

        public GroupServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PaginatedOutputServices<GroupModel>> GetGroup(int? page)
        {
            var response = await httpClient.GetAsync($"/Group/GetGroup?page={page}");
            var result = await response.Content.ReadFromJsonAsync<PaginatedOutputServices<GroupModel>>();
            return result;
        }

        public async Task<ReturnGenericData<ReturnGroup>> GetGroupDetails(int id)
        {
            var response = await httpClient.GetAsync($"/Group/GetGroupDetails?id={id}");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnGroup>>();
            return result;
        }

        public async Task<ReturnGenericStatus> GroupCreate(GroupModel groupModel)
        {
            var response = await httpClient.PostAsJsonAsync("/Group/GroupCreate", groupModel);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
            return result;
        }

        public async Task<ReturnGenericStatus> GroupDeleteConfirmed(int id)
        {
            var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>($"/Group/GroupDelete?id={id}", null);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
            return result;
        }

        public async Task<ReturnGenericStatus> GroupEdit(int id, GroupModel groupModel)
        {
            var response = await httpClient.PostAsJsonAsync($"/Group/GroupEdit?id={id}", groupModel);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
            return result;
        }

        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintMatrix()
        {
            var response = await httpClient.GetAsync("/Group/GroupDownloadMatrix");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnDownloadPDF>>();
            return result;
        }

        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintReport()
        {
            var response = await httpClient.GetAsync("/Group/GroupDownload");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<ReturnDownloadPDF>>();
            return result;
        }
    }
}
