using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Infrastructure.Repositories
{
    public class EmailTemplateRepositories : IEmailTemplateRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;

        public EmailTemplateRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }
        public async Task<PaginatedOutput<EmailTemplateModel>> GetEmailTemplateList(int page = 1)
        {
            try
            {
                IQueryable<EmailTemplateModel> EmailTemplateModels = _dBContext.EmailTemplateModels.OrderBy(e => e.EmailFor);

                PaginatedList<EmailTemplateModel> Paginate = await PaginatedList<EmailTemplateModel>.CreateAsync(EmailTemplateModels, page);
                var data = new PaginatedOutput<EmailTemplateModel>(Paginate);

                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<EmailTemplateModel>?> GetEmailTemplate(int? id)
        {
            ReturnGenericStatus status = new();
            EmailTemplateModel Data = new();
            try
            {
                if (id == null)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    Data = await _dBContext.EmailTemplateModels.FirstOrDefaultAsync(m => m.Id == id);
                    if (Data == null)
                        status = _commonClass.MsgError("No data found");
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            ReturnGenericData<EmailTemplateModel> r = new()
            {
                StatusCode = status.StatusCode,
                StatusMessage = status.StatusMessage,
                Data = Data,
            };
            return r;
        }
        public async Task<ReturnGenericStatus> SaveEmailTemplate(EmailTemplateModel data)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Email Template was successfully saved");
            try
            {
                if (data.Id == 0)
                {
                    if (!EmailTemplateModelsExists(data.EmailFor))
                    {
                        _dBContext.Add(data);
                        await _dBContext.SaveChangesAsync();
                    }
                    else
                    {
                        status = _commonClass.MsgError("Email For Already Exist. Please Change.");
                    }
                }
                else
                {
                    if (EmailTemplateModelsExists(data.EmailFor, data.Id))
                    {
                        status = _commonClass.MsgError("Email For Already Exist. Please Change.");
                    }
                    else
                    {
                        var branchModel = await _dBContext.EmailTemplateModels.FindAsync(data.Id);
                        if (branchModel == null)
                            status = _commonClass.MsgError("No data found");
                        else
                        {
                            branchModel.EmailFor = data.EmailFor;
                            branchModel.EmailBody = data.EmailBody;
                            branchModel.EmailSubjest = data.EmailSubjest;
                            try
                            {
                                _dBContext.Update(branchModel);
                                await _dBContext.SaveChangesAsync();
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        public async Task<ReturnGenericStatus> DeleteEmailTemplate(int id)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Email Template successfully deleted");
            try
            {
                var EmailTemplateModels = await _dBContext.EmailTemplateModels.FindAsync(id);
                if (EmailTemplateModels == null)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    _dBContext.EmailTemplateModels.Remove(EmailTemplateModels);
                    await _dBContext.SaveChangesAsync();
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        private bool EmailTemplateModelsExists(string EmailFor, int? id = null)
        {
            var ICount = _dBContext.EmailTemplateModels.Where(e => e.EmailFor == EmailFor && (id ?? 0) != e.Id).Count();
            return ICount > 0;
        }
    }
}
