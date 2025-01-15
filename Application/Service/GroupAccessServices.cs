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
    public class GroupAccessServices : IGroupAccessServices
    {
        private readonly HttpClient httpClient;

        public GroupAccessServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }   

        public async Task<ReturnGenericData<GroupAccessModel>> GetGroupAccess(int id)
        {
            var response = await httpClient.GetAsync($"/GroupAccess/GroupAccess?id={id}");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<GroupAccessModel>>();
            return result;
        }
        public async Task<ReturnGenericList<SelectListItem>> GetGroupAccessMenuList()
        {
            var response = await httpClient.GetAsync($"/GroupAccess/GetMenuList");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericList<SelectListItem>>();
            return result;
        }
        public async Task<ReturnGenericStatus> GroupAccessEdit(GroupAccessModel groupAccessModel)
        {
            var response = await httpClient.PostAsJsonAsync($"/GroupAccess/GroupAccessEdit", groupAccessModel);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
            return result;
        }
        public async Task<ReturnGenericData<GroupAccessModel>> GroupAccessDetails(int id)
        {
            var response = await httpClient.GetAsync($"/GroupAccess/GroupAccessDetails?id={id}");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<GroupAccessModel>>();
            return result;
        }

        public Task<ReturnGenericStatus> GroupAccessCreate(GroupAccessModel groupAccessModel)
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnGenericStatus> GroupAccessDeleteConfirmed(int id)
        {
            var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>($"/GroupAccess/GroupAccesDelete?id={id}", null);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
            return result;
        }

        public async Task<PaginatedOutputServices<GroupAccessModel>> GetGroupAccessList(int? page)
        {
            var response = await httpClient.GetAsync($"/GroupAccess/GetGroupAccessList?page={page}");
            var result = await response.Content.ReadFromJsonAsync<PaginatedOutputServices<GroupAccessModel>>();
            return result;
        }
    }
}
