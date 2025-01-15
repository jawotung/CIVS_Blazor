using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Service.Interfaces
{
    public interface IEmailTemplateServices
    {
        Task<PaginatedOutputServices<EmailTemplateModel>> GetEmailTemplateList(string sKey = "", int page = 1);
        Task<ReturnGenericData<EmailTemplateModel>> GetEmailTemplate(int? id);
        Task<ReturnGenericDictionary> SaveEmailTemplate(EmailTemplateModel data);
        Task<ReturnGenericStatus> DeleteEmailTemplate(int id);
        Task<ReturnGenericData<ReturnDownloadPDF>> PrintEmailTemplate(string sKey = "");
    }
}
