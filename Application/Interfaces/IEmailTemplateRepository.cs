using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Interfaces
{
    public interface IEmailTemplateRepository
    {
        Task<PaginatedOutput<EmailTemplateModel>> GetEmailTemplateList(int page = 1);
        Task<ReturnGenericData<EmailTemplateModel>> GetEmailTemplate(int? id);
        Task<ReturnGenericStatus> SaveEmailTemplate(EmailTemplateModel data);
        Task<ReturnGenericStatus> DeleteEmailTemplate(int id);
    }
}
