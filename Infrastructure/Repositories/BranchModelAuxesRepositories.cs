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
    public class BranchModelAuxesRepositories : IBranchModelAuxesRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;

        public BranchModelAuxesRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }
        public async Task<PaginatedOutput<BranchModelAux>> GetBranchModelAuxList(string sKey = "", int page = 1)
        {
            try
            {
                IQueryable<BranchModelAux> branchModels = _dBContext.BranchModelAuxes.OrderBy(b => b.BranchCode);
                if (string.IsNullOrEmpty(sKey))
                    branchModels = _dBContext.BranchModelAuxes.OrderBy(b => b.BranchCode);
                else
                    branchModels = _dBContext.BranchModelAuxes.Where(e => e.BranchCode.Contains(sKey)).OrderBy(b => b.BranchCode);

                PaginatedList<BranchModelAux> Paginate = await PaginatedList<BranchModelAux>.CreateAsync(branchModels, page);
                var data = new PaginatedOutput<BranchModelAux>(Paginate);

                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<BranchModelAux>?> GetBranchModelAux(int? id)
        {
            ReturnGenericStatus status = new();
            BranchModelAux Data = new();
            try
            {
                if (id == null)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    Data = await _dBContext.BranchModelAuxes.FirstOrDefaultAsync(m => m.Id == id);
                    if (Data == null)
                        status = _commonClass.MsgError("No data found");
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            ReturnGenericData<BranchModelAux> r = new()
            {
                StatusCode = status.StatusCode,
                StatusMessage = status.StatusMessage,
                Data = Data,
            };
            return r;
        }
        public async Task<ReturnGenericStatus> SaveBranchModelAux(BranchModelAux data)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Data was successfully saved");
            try
            {
                if (data.Id == 0)
                {
                    if (data.BranchBuddyCode == null)
                        data.BranchBuddyCode = data.BranchCode;
                    _dBContext.Add(data);
                    await _dBContext.SaveChangesAsync();
                }
                else
                {
                    var branchModelAux = await _dBContext.BranchModelAuxes.FindAsync(data.Id);
                    if (branchModelAux == null)
                        status = _commonClass.MsgError("No data found");
                    else
                    {
                        branchModelAux.BranchBuddyCode = data.BranchBuddyCode;
                        branchModelAux.BranchCode = data.BranchCode;
                        branchModelAux.BranchBcp = data.BranchBcp;
                        try
                        {
                            _dBContext.Update(branchModelAux);
                            await _dBContext.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            throw;
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
        public async Task<ReturnGenericStatus> DeleteBranchModelAux(int id)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Data successfully deleted");
            try
            {
                var branchModelAux = await _dBContext.BranchModelAuxes.FindAsync(id);
                if (branchModelAux == null)
                    status = _commonClass.MsgError("No data found");

                _dBContext.BranchModelAuxes.Remove(branchModelAux);
                await _dBContext.SaveChangesAsync();
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintBranchModelAux(string sKey = "")
        {
            ReturnGenericData<ReturnDownloadPDF> status = new ReturnGenericData<ReturnDownloadPDF>();
            try
            {
                IQueryable<BranchModelAux> chekReportDetail = _dBContext.BranchModelAuxes.OrderBy(b => b.BranchCode);
                if (string.IsNullOrEmpty(sKey))
                    chekReportDetail = _dBContext.BranchModelAuxes.OrderBy(b => b.BranchCode);
                else
                    chekReportDetail = _dBContext.BranchModelAuxes.Where(e => e.BranchCode.Contains(sKey)).OrderBy(b => b.BranchCode);

                List<BranchModelAux> chekReportDetailList = await chekReportDetail.ToListAsync();
                Dictionary<int, List<string>> DataList = chekReportDetail
                                                       .ToDictionary(x => x.Id, x => (string.Format("{0}|{1}|{2}",
                                                                                     x.BranchCode, x.BranchCode, x.BranchBuddyCode, x.BranchBcp)
                                                                                     .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                        );

                string filename = string.Format("attachment; filename=\"BranchListReport{0}.pdf\"", DateTime.Now.ToString("MMddyyyy"));
                var contentDisposition = filename;// "attachment; filename=\"UserReport.pdf\"";

                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userName = userClaims.DisplayName;

                PDFReport pDFReport = new PDFReport();
                string TitleHeader = "Branch Buddy BCP Report";
                List<int> widthList = new List<int> { 300, 300, 220 };
                List<string> Header = "Branch Code|Branch Buddy Code|Branch Bcp"
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                byte[] data = pDFReport.GeneratePDF(
                                          DataList,
                                          "BranchBuddyBCPReport", userName, userId,
                                          TitleHeader,
                                          Header,
                                          widthList
                                          );
                string base64String = Convert.ToBase64String(data);
                string fileName = string.Format("BranchBuddyBCPListReport{0}.pdf", DateTime.Now.ToString("MMddyyyy"));

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

    }
}
