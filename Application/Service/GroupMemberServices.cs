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
    public class GroupMemberServices : IGroupMemberServices
    {
        private readonly HttpClient httpClient;

        public GroupMemberServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ReturnGenericData<GroupMemberModel>> GetGroupMember(int id)
        {
            var response = await httpClient.GetAsync($"/GroupMember/GetGroupMember?id={id}");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<GroupMemberModel>>();
            return result;
        }

        public async Task<PaginatedOutputServices<GroupMemberModel>> GetGroupMemberList(int? page)
        {
            var response = await httpClient.GetAsync($"/GroupMember/GetGroupMemberList?page={page}");
            var result = await response.Content.ReadFromJsonAsync<PaginatedOutputServices<GroupMemberModel>>();
            return result;
        }

        public async Task<ReturnGenericList<SelectListItem>> GetUserTypeList()
        {
            var response = await httpClient.GetAsync($"/GroupMember/GetUserTypeList");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericList<SelectListItem>>();
            return result;
        }

        public Task<ReturnGenericStatus> GroupMemberCreate(GroupMemberModel groupMemberModel)
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnGenericStatus> GroupMemberDeleteConfirmed(int id)
        {
            var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>($"/GroupMember/GroupMemberDelete?id={id}", null);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
            return result;
        }

        public async Task<ReturnGenericData<GroupMemberModel>> GroupMemberDetails(int id)
        {
            var response = await httpClient.GetAsync($"/GroupMember/GroupMemberDetails?id={id}");
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericData<GroupMemberModel>>();
            return result;
        }

        public async Task<ReturnGenericStatus> GroupMemberEdit(GroupMemberModel groupMemberModel)
        {
            var response = await httpClient.PostAsJsonAsync($"/GroupMember/GroupMemberEdit", groupMemberModel);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
            return result;
        }
    }
}
