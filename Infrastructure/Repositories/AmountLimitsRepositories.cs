using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repositories
{
    public class AmountLimitsRepositories : IAmountLimitsRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;

        public AmountLimitsRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass,IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }
        public async Task<PaginatedOutput<AmountLimitsModel>> GetAmountLimitsList(int page = 1)
        {
            try
            {
                IQueryable<AmountLimitsModel> AmountLimitsModels = _dBContext.AmountLimitsModels.OrderBy(b => b.AmountLimitsCode);
                PaginatedList<AmountLimitsModel> Paginate = await PaginatedList<AmountLimitsModel>.CreateAsync(AmountLimitsModels, page);
                var data = new PaginatedOutput<AmountLimitsModel>(Paginate);

                foreach (var x in data.Data)
                {
                    x.AllowedAction = ActionDesc(x.AllowedAction);
                    x.SelectedActions = x.AllowedAction.Split(',');
                }
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<AmountLimitsModel>> GetAmountLimits(int? id)
        {
            ReturnGenericStatus status = new();
            AmountLimitsModel Data = new();
            try
            {
                if (id == null)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    Data = await _dBContext.AmountLimitsModels.FirstOrDefaultAsync(m => m.Id == id);
                    if (Data == null)
                        status = _commonClass.MsgError("No data found");
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            ReturnGenericData<AmountLimitsModel> r = new()
            {
                StatusCode = status.StatusCode,
                StatusMessage = status.StatusMessage,
                Data = Data,
            };
            return r;
        }
        public async Task<ReturnGenericStatus> SaveAmountLimits(AmountLimitsModel data)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Amount limits was successfully saved");
            try
            {
                if (data.Id == 0)
                {
                    if (!AmountLimitsModelExists(data.AmountLimitsCode))
                    {
                        data.AllowedAction = string.Join(",", data.SelectedActions);
                        data.Isdeleted = false;
                        _dBContext.Add(data);
                        await _dBContext.SaveChangesAsync();
                    }
                    else
                    {
                        status = _commonClass.MsgError("Amount Limits Code Already Exist. Please Change.");
                    }
                }
                else
                {
                    if (AmountLimitsModelExists(data.AmountLimitsCode, data.Id))
                    {
                        status = _commonClass.MsgError("Amount Limits Code Already Exist. Please Change.");
                    }
                    else
                    {
                        var AmountLimitsModel = await _dBContext.AmountLimitsModels.FindAsync(data.Id);
                        if (AmountLimitsModel == null)
                            status = _commonClass.MsgError("No data found");
                        else if ((AmountLimitsModel.Isdeleted ?? false))
                            status = _commonClass.MsgError("No data found");
                        else
                        {
                            AmountLimitsModel.AmountLimitsCode = data.AmountLimitsCode;
                            AmountLimitsModel.AmountLimitsDesc = data.AmountLimitsDesc;
                            AmountLimitsModel.MaxAmountLimit = data.MaxAmountLimit;
                            AmountLimitsModel.AllowedAction = string.Join(",", data.SelectedActions);
                            try
                            {
                                _dBContext.Update(AmountLimitsModel);
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
        public async Task<ReturnGenericStatus> DeleteAmountLimits(int id)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Data successfully deleted");
            try
            {
                var AmountLimitsModel = await _dBContext.AmountLimitsModels.FindAsync(id);
                if (AmountLimitsModel == null)
                    status = _commonClass.MsgError("No data found");
                else if ((AmountLimitsModel.Isdeleted ?? false))
                    status = _commonClass.MsgError("No data found");
                else
                {
                    AmountLimitsModel.Isdeleted = true;
                    _dBContext.Update(AmountLimitsModel);
                    await _dBContext.SaveChangesAsync();
                }

            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintAmountLimits()
        {
            ReturnGenericData<ReturnDownloadPDF> status = new ReturnGenericData<ReturnDownloadPDF>();
            try
            {
                List<AmountLimitsModel> ReportData = await _dBContext.AmountLimitsModels.ToListAsync();
                Dictionary<int, List<string>> DataList = ReportData
                                                       .ToDictionary(x => x.Id, x => (string.Format("{0}|{1}|{2}|{3}|{4}",
                                                                                     x.AmountLimitsCode, x.AmountLimitsDesc, x.MaxAmountLimit, ActionDesc(x.AllowedAction).Replace(",", "\n"), x.Isdeleted)
                                                                                     .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                        );

                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userName = userClaims.DisplayName;

                PDFReport pDFReport = new PDFReport();
                string TitleHeader = "Amount Limits Report";
                List<int> widthList = new List<int> { 150, 250, 150, 300, 200 };
                List<string> Header = "Amount Limits Code|Amount Limits Desc|Max Amount Limit|Acctions|Deleted"
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                int ColumnCustomHeight = 3;
                byte[] data = pDFReport.GeneratePDF(
                                          DataList,
                                          "AmountLimitsReport", userName, userId,
                                          TitleHeader,
                                          Header,
                                          widthList,
                                          ColumnCustomHeight
                                          );
                string base64String = Convert.ToBase64String(data);
                string fileName = string.Format("AmountLimitsReport{0}.pdf", DateTime.Now.ToString("MMddyyyy"));

                var printReportResult = new ReturnDownloadPDF
                {
                    PdfDataBase64 = base64String,
                    FileName = fileName
                };
                status.Data = printReportResult;
                status.StatusCode = "00";
                status.StatusMessage = "SUCCESS";
            }
            catch(Exception ex)
            {
                ReturnGenericStatus err = _commonClass.MsgError(ex.Message);
                status.StatusCode = err.StatusCode;
                status.StatusMessage = err.StatusMessage;
            }
            return status;
        }
        public Task<ReturnGenericDropdown> GetAllowedAction()
        {
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            ReturnGenericStatus status = new();
            try
            {
                x.Data = _commonClass.GetActionList();
                status = _commonClass.MsgSuccess("");

            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            x.StatusMessage = status.StatusMessage;
            x.StatusCode = status.StatusCode;
            return Task.FromResult(x);
        }
        private bool AmountLimitsModelExists(string AmountLimitsCode, int? id = null)
        {
            var ICount = _dBContext.AmountLimitsModels.Where(e => e.AmountLimitsCode == AmountLimitsCode && (e.Isdeleted ?? false) == false && (id ?? 0) != e.Id).Count();
            return  ICount > 0;
        }
        private string ActionDesc(string str)
        {
            return _commonClass.GetActionDesc(str);
        }
    }
}
