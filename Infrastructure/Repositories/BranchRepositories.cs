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
    public class BranchRepositories : IBranchRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;

        public BranchRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }
        public async Task<PaginatedOutput<BranchModel>> GetBranchList(string sKey = "", int page = 1)
        {
            try
            {
                IQueryable<BranchModel> branchModels = _dBContext.BranchModels.OrderBy(b => b.BranchDesc);
                if (string.IsNullOrEmpty(sKey))
                    branchModels = _dBContext.BranchModels.OrderBy(b => b.BranchDesc);
                else
                    branchModels = _dBContext.BranchModels.Where(e => e.BranchDesc.Contains(sKey)).OrderBy(b => b.BranchDesc);

                PaginatedList<BranchModel> Paginate = await PaginatedList<BranchModel>.CreateAsync(branchModels, page);
                var data = new PaginatedOutput<BranchModel>(Paginate);

                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<BranchModel>> GetBranch(int? id)
        {
            ReturnGenericStatus status = new();
            BranchModel Data = new();
            try
            {
                if (id == null)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    Data = await _dBContext.BranchModels.FirstOrDefaultAsync(m => m.Id == id);
                    if (Data == null)
                        status = _commonClass.MsgError("No data found");
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            ReturnGenericData<BranchModel> r = new()
            {
                StatusCode = status.StatusCode,
                StatusMessage = status.StatusMessage,
                Data = Data,
            };
            return r;
        }
        public async Task<ReturnGenericStatus> SaveBranch(BranchModel data)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Branch was successfully saved");
            try
            {
                if (data.Id == 0)
                {
                    if (!BranchModelExists(data.BranchCode))
                    {
                        _dBContext.Add(data);
                        await _dBContext.SaveChangesAsync();

                    }
                    else
                    {
                        status = _commonClass.MsgError("Branch Code Already Exist. Please Change.");
                    }
                }
                else
                {
                    if (BranchModelExists(data.BranchCode, data.Id))
                    {
                        status = _commonClass.MsgError("Branch Code Already Exist. Please Change.");
                    }
                    else
                    {
                        var branchModel = await _dBContext.BranchModels.FindAsync(data.Id);
                        if (branchModel == null)
                            status = _commonClass.MsgError("No data found");
                        if (branchModel.Isdeleted)
                            status = _commonClass.MsgError("No data found");
                        else
                        {
                            branchModel.BranchDesc = data.BranchDesc;
                            branchModel.BranchEmail = data.BranchEmail;
                            branchModel.BranchBrstn = data.BranchBrstn;
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
        public async Task<ReturnGenericStatus> DeleteBranch(int id)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Branch successfully deleted");
            try
            {
                var branchModel = await _dBContext.BranchModels.FindAsync(id);
                if (branchModel == null)
                    status = _commonClass.MsgError("No data found");
                else if (branchModel.Isdeleted)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    branchModel.Isdeleted = true;
                    _dBContext.Update(branchModel);
                    await _dBContext.SaveChangesAsync();
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintBranch(string sKey = "")
        {
            ReturnGenericData<ReturnDownloadPDF> status = new ReturnGenericData<ReturnDownloadPDF>();
            try
            {
                IQueryable<BranchModel> chekReportDetail = _dBContext.BranchModels.OrderBy(b => b.BranchDesc);
                switch (sKey ?? "")
                {
                    case "":
                        chekReportDetail = _dBContext.BranchModels.OrderBy(b => b.BranchDesc);
                        break;
                    default:
                        chekReportDetail = _dBContext.BranchModels.Where(e => e.BranchDesc.StartsWith((sKey ?? ""))).OrderBy(b => b.BranchDesc); ;
                        break;
                }
                List<BranchModel> chekReportDetailList = await chekReportDetail.ToListAsync();
                Dictionary<int, List<string>> DataList = chekReportDetail
                                                       .ToDictionary(x => x.Id, x => (string.Format("{0}|{1}|{2}|{3}",
                                                                                     x.BranchCode, x.BranchDesc, x.BranchEmail, x.Isdeleted)
                                                                                     .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                        );

                string filename = string.Format("attachment; filename=\"BranchListReport{0}.pdf\"", DateTime.Now.ToString("MMddyyyy"));
                var contentDisposition = filename;// "attachment; filename=\"UserReport.pdf\"";

                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userName = userClaims.DisplayName;

                PDFReport pDFReport = new PDFReport();
                string TitleHeader = "Branch List Report";
                List<int> widthList = new List<int> { 100, 220, 250, 100 };
                List<string> Header = "Branch Code|Branch Desc|Branch Email|Deleted"
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                byte[] data = pDFReport.GeneratePDF(
                                          DataList,
                                          "BranchListReport", userName, userId,
                                          TitleHeader,
                                          Header,
                                          widthList
                                          );
                string base64String = Convert.ToBase64String(data);
                string fileName = string.Format("BranchListReport{0}.pdf", DateTime.Now.ToString("MMddyyyy"));

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
        private bool BranchModelExists(string BranchCode, int? id = null)
        {
            var ICount = _dBContext.BranchModels.Where(e => e.BranchCode == BranchCode && e.Isdeleted == false && (id ?? 0) != e.Id).Count();
            return ICount > 0;
        }
    }
}
