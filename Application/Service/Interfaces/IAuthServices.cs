using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Interfaces
{
    public interface IAuthServices
    {
        Task<ReturnLoginCredentials> ValidateCredentials(ParamLoginCredentials value);
        Task<ReturnGenericStatus> TagInactiveUsers(int? mode);
        Task<ReturnGenericStatus> ClearAllSession();
        Task<ReturnGenericStatus> AuthLogout(string? sMode);
    }
}
