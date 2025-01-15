using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IAmountLimitsRepository
    {
        Task<PaginatedOutput<AmountLimitsModel>> GetAmountLimitsList(int page = 1);
        Task<ReturnGenericData<AmountLimitsModel>> GetAmountLimits(int? id);
        Task<ReturnGenericStatus> SaveAmountLimits(AmountLimitsModel data);
        Task<ReturnGenericStatus> DeleteAmountLimits(int id);
        Task<ReturnGenericDropdown> GetAllowedAction();
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintAmountLimits();

    }
}
