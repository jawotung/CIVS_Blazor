using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebAPI;

namespace Infrastructure.Repositories
{
    public class RolePolicyService : IRolePolicyService
    {
        private readonly AppDbContext _dBContext;

        public RolePolicyService(AppDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<Dictionary<string, List<string>>> GetRolePolicies()
        {
            var policies = new Dictionary<string, List<string>>();

            var policyEntities = await _dBContext.UserTypeModels.ToListAsync();

            foreach (var policy in policyEntities)
            {
                if (!policies.ContainsKey(policy.UserTypeDesc))
                {
                    policies[policy.UserTypeDesc] = new List<string>();
                }
                policies[policy.UserTypeDesc].Add(policy.UserTypeCode);
            }

            return policies;
        }
    }
}
