using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Infrastructure.Repositories
{
    public class RejectReasonRepositories : IRejectReasonRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;

        public RejectReasonRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }
        public async Task<PaginatedOutput<RejectReasonModel>> GetRejectReasonList(int page = 1)
        {
            try
            {
                IQueryable<RejectReasonModel> RejectReasonModels = _dBContext.RejectReasonModels.OrderBy(b => b.RejectReasonDesc);
                PaginatedList<RejectReasonModel> Paginate = await PaginatedList<RejectReasonModel>.CreateAsync(RejectReasonModels, page);
                var data = new PaginatedOutput<RejectReasonModel>(Paginate);

                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<RejectReasonModel>> GetRejectReason(int? id)
        {
            ReturnGenericStatus status = new();
            RejectReasonModel Data = new();
            try
            {
                if (id == null)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    Data = await _dBContext.RejectReasonModels.FirstOrDefaultAsync(m => m.Id == id);
                    if (Data == null)
                        status = _commonClass.MsgError("No data found");
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            ReturnGenericData<RejectReasonModel> r = new()
            {
                StatusCode = status.StatusCode,
                StatusMessage = status.StatusMessage,
                Data = Data,
            };
            return r;
        }
        public async Task<ReturnGenericStatus> SaveRejectReason(RejectReasonModel data)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Reject reason was successfully saved");
            try
            {
                if (data.Id == 0)
                {
                    data.RejectReasonCode = GetRejectCode();
                    if (!RejectReasonModelExists(data.RejectReasonCode))
                    {
                        _dBContext.Add(data);
                        await _dBContext.SaveChangesAsync();
                    }
                    else
                    {
                        status = _commonClass.MsgError("Reject Reason Code Already Exist. Please Change.");
                    }
                }
                else
                {
                    if (RejectReasonModelExists(data.RejectReasonCode, data.Id))
                    {
                        status = _commonClass.MsgError("Reject Reason Code Already Exist. Please Change.");
                    }
                    else
                    {
                        var RejectReasonModel = await _dBContext.RejectReasonModels.FindAsync(data.Id);
                        if (RejectReasonModel == null)
                            status = _commonClass.MsgError("No data found");
                        if (RejectReasonModel.Isdeleted)
                            status = _commonClass.MsgError("No data found");
                        else
                        {
                            RejectReasonModel.RejectReasonDesc = data.RejectReasonDesc;
                            try
                            {
                                _dBContext.Update(RejectReasonModel);
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
        public async Task<ReturnGenericStatus> DeleteRejectReason(int id)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Data successfully deleted");
            try
            {
                var RejectReasonModel = await _dBContext.RejectReasonModels.FindAsync(id);
                if (RejectReasonModel == null)
                    status = _commonClass.MsgError("No data found");
                if (RejectReasonModel.Isdeleted)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    RejectReasonModel.Isdeleted = true;
                    _dBContext.Update(RejectReasonModel);
                    await _dBContext.SaveChangesAsync();
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintRejectReason()
        {
            ReturnGenericData<ReturnDownloadPDF> status = new ReturnGenericData<ReturnDownloadPDF>();
            try
            {
                List<RejectReasonModel> ReportData = await _dBContext.RejectReasonModels.ToListAsync();
                Dictionary<int, List<string>> DataList = ReportData
                                                       .ToDictionary(x => x.Id, x => (string.Format("{0}|{1}|{2}",
                                                                                     x.RejectReasonCode, x.RejectReasonDesc, x.Isdeleted)
                                                                                     .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                        );

                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userName = userClaims.DisplayName;

                PDFReport pDFReport = new PDFReport();
                string TitleHeader = "Reject Reason List Report";
                List<int> widthList = new List<int> { 150, 300, 100 };
                List<string> Header = "Reject Reason Code|Reject Reason Desc|Deleted"
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                byte[] data = pDFReport.GeneratePDF(
                                          DataList,
                                          "RejectReasonListReport", userName, userId,
                                          TitleHeader,
                                          Header,
                                          widthList
                                          );
                string base64String = Convert.ToBase64String(data);
                string fileName = string.Format("RejectReasonListReport{0}.pdf", DateTime.Now.ToString("MMddyyyy"));

                var printReportResult = new ReturnDownloadPDF
                {
                    PdfDataBase64 = base64String,
                    FileName = fileName
                };
                status.Data = printReportResult;
                status.StatusCode = "00";
                status.StatusMessage = "SUCCESS";
            }
            catch (Exception ex)
            {
                ReturnGenericStatus err = _commonClass.MsgError(ex.Message);
                status.StatusCode = err.StatusCode;
                status.StatusMessage = err.StatusMessage;
            }
            return status;
        }
        private bool RejectReasonModelExists(string RejectReasonCode, int? id = null)
        {
            var ICount = _dBContext.RejectReasonModels.Where(e => e.RejectReasonCode == RejectReasonCode && e.Isdeleted == false && (id ?? 0) != e.Id).Count();
            return ICount > 0;
        }

        private string GetRejectCode()
        {
            var rejMod = _dBContext.RejectReasonModels;
            int id = 1;
            if (rejMod.Any())
            {
                id = rejMod.OrderBy(i => i.Id).Last().Id + 1;
            }
            return string.Format("RejCde{0}", id.ToString("000#"));
        }
    }
}
