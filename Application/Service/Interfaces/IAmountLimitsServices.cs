using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IAmountLimitsServices
    {
        Task<PaginatedOutputServices<AmountLimitsModel>> GetAmountLimitsList(string sKey = "", int page = 1);
        Task<ReturnGenericData<AmountLimitsModel>> GetAmountLimits(int? id);
        Task<ReturnGenericDictionary> SaveAmountLimits(AmountLimitsModel data);
        Task<ReturnGenericStatus> DeleteAmountLimits(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintAmountLimits(string sKey = "");
        Task<ReturnGenericDropdown> GetAllowedAction();
    }
}
