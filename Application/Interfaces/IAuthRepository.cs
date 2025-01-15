using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<ReturnLoginCredentials> ValidateCredentials(ParamLoginCredentials value);
        Task<ReturnGenericStatus> SaveApiAuthentication(ParamSaveApiAuthentication value);
        Task<ReturnGenericStatus> AuthLogout(string? sMode);
        Task<ReturnGenericStatus> TagInactiveUsers(int? mode);
        Task<ReturnGenericStatus> ClearAllSession();
    }
}
