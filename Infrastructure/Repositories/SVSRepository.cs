using Application.Interfaces;
using Application.Models;
using Azure.Core;
using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebAPI;

namespace Infrastructure.Repositories
{
    public class SVSRepository : ISVSRepository
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        public SVSRepository(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<ReturnGenericData<ReturnSignatureInquiry>> SignatureInquiry(ParamSignatureInquiry value)
        {
            var obj = new ReturnGenericData<ReturnSignatureInquiry>();
            obj.StatusCode = "01";
            try
            {
                var jsonRequest = JsonSerializer.Serialize(value);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_config["SignatureInquiryUrl"], content);

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var signatureInquiryResponse = JsonSerializer.Deserialize<ReturnSignatureInquiry>(jsonResponse);

                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
                obj.Data = signatureInquiryResponse;
            }
            catch(Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }
    }
}
