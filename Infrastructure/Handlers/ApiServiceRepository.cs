using Application.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
using Application.Interfaces;

namespace Infrastructure.Handlers
{
    public class ApiServiceRepository : IApiServiceRepository
    {
        private readonly AppDbContext _dBContext;

        public ApiServiceRepository(AppDbContext context)
        {
            _dBContext = context;
        }

        public async Task<bool> Authenticate(string userId, string accessToken)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                var oneDayAgo = DateTime.Now.AddDays(-1);
                var now = DateTime.Now;

                var result = await _dBContext.UserAuthenticationModels
                    .Where(u => u.IsValid == true && 
                                u.UserId == userId && 
                                u.CreatedAt >= oneDayAgo && 
                                u.AccessToken == accessToken && u.AccessTokenExpiry > now)
                    .ToListAsync();
                if (result.Count == 0)
                {
                    obj.StatusMessage = "Unauthorized";
                    return false;
                }
                else
                {
                    obj.StatusCode = "00";
                    obj.StatusMessage = "SUCCESS";
                    return true;
                }
            }
            catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;
                return false;
            }
            //return obj;
        }
    }
}
