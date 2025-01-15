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
using Newtonsoft.Json.Linq;

namespace Application.Service
{
    public class AuthServices : IAuthServices
    {
        private readonly HttpClient httpClient;

        public AuthServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ReturnLoginCredentials> ValidateCredentials(ParamLoginCredentials value)
        {
            var response = await httpClient.PostAsJsonAsync("/Auth/ValidateCredentials", value);
            var result = await response.Content.ReadFromJsonAsync<ReturnLoginCredentials>();
            return result;
        }
        public async Task<ReturnGenericStatus> TagInactiveUsers(int? mode)
        {
            var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>($"/Auth/TagInactiveUsers?mode={mode}", null);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
            return result;
        }

        public async Task<ReturnGenericStatus> ClearAllSession()
        {
            var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>($"/Auth/ClearAllSession", null);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
            return result;
        }

        public async Task<ReturnGenericStatus> AuthLogout(string? sMode)
        {
            var response = await httpClient.PostAsJsonAsync<ReturnGenericStatus>($"/Auth/Logout?sMode={sMode}", null);
            var result = await response.Content.ReadFromJsonAsync<ReturnGenericStatus>();
            return result;
        }
    }
}
