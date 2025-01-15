using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Azure;
using System.Net.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using PdfSharp;

namespace Infrastructure.Repositories
{
    public class InwardClearingChequeReportRepositories : IInwardClearingChequeReportRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;
        public InwardClearingChequeReportRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass,
            IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }
        public async Task<PaginatedOutput<InwardClearingReportModel>> GetReportList(string Key, int page = 1, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null)
        {
            try
            {
                List<InwardClearingChequeDetailsModel> chekReportDetailList = new List<InwardClearingChequeDetailsModel>();
                IQueryable<InwardClearingChequeDetailsModel> res = await QueryableReportList(Key, dateFromFilter, dateToFilter, BSRTNFilter);
                chekReportDetailList = PaginatedList<InwardClearingChequeDetailsModel>.Create(res, page);
                List<InwardClearingReportModel> inwardClearingReportList = await FGetReportList(chekReportDetailList);
                PaginatedList<InwardClearingReportModel> data = PaginatedList<InwardClearingReportModel>.Create(inwardClearingReportList, 1);
                PaginatedOutput<InwardClearingReportModel> x = new PaginatedOutput<InwardClearingReportModel>(new PaginatedList<InwardClearingReportModel>(data, res.Count(), page, 10));
                return x;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericDropdown> GetBuddyBranches()
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            List<SelectListItem> result = new List<SelectListItem>();
            try
            {
                var userClaims = _userClaims.GetClaims();
                string userBranch = userClaims.BranchCode ?? "",
                       userType = userClaims.UserType ?? "";

                userBranch = userType.Contains("Branch") ? userBranch : "";
                if (!userBranch.Equals(""))
                {
                    string[] branchBuddy = _commonClass.GetBuddyBranches(userBranch);

                    List<string> branchCode = new List<string>();
                    branchCode.Add(userBranch);
                    if (branchBuddy.Length > 0)
                        branchCode.AddRange(branchBuddy);

                    result = await _dBContext.BranchModels
                        .Where(b => b.BranchBrstn.ToInt() > 0 && (branchCode.Contains(b.BranchCode)))
                        .Select(b => new SelectListItem  { Value = b.BranchBrstn, Text = (b.BranchCode == branchCode[0] ? b.BranchDesc : $"{b.BranchDesc} (Buddy)") })//BranchDesc = string.Format("{0} {1}", b.BranchBRSTN, b.BranchDesc)})
                        .OrderBy(b => b.Value).ToListAsync<SelectListItem>();
                    x.Data = result;
                }
                status = _commonClass.MsgSuccess("");
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            x.StatusMessage = status.StatusMessage;
            x.StatusCode = status.StatusCode;
            return x;
            
        }
        public async Task<ReturnGenericData<MCCheckDetailModel>> GetMCDetails(string acctAmt)
        {
            ReturnGenericData<MCCheckDetailModel> status = new ReturnGenericData<MCCheckDetailModel>();
            try
            {
                if (acctAmt.IsNull())
                {
                    status.StatusCode = "01";
                    status.StatusMessage = "Invalid Request";
                }
                else
                {
                    var vParam = acctAmt.Split('_');

                    var chekReportDetail = _dBContext.InwardClearingChequeDetailsModels;

                    var detailCheck = chekReportDetail.FirstOrDefault(e => e.AccountNumber == vParam[0]
                                        && e.CheckNumber == vParam[1]);

                    MCCheckDetailModel checkDetailModel = new MCCheckDetailModel()
                    {
                        AccountNumber = detailCheck.AccountNumber,
                        CheckNumber = detailCheck.CheckNumber,
                        CheckAmount = detailCheck.CheckAmount.ToString("#,##0.00"),
                        BRSTN = detailCheck.Brstn.Trim(),
                        EffectivityDate = detailCheck.EffectivityDate,
                        CheckStatus = string.Format("{0} {1} {2}",
                                        _commonClass.GetStatusDesc(detailCheck.CheckStatus), //detailCheck.CheckStatus == "Open" ? "Open" : GetStatusList("", "").Find(l => l.Value == detailCheck.CheckStatus).Text,
                                        detailCheck.CheckStatus == "ReAssign" ? _commonClass.GetBranchOfAssignmentDesc(detailCheck.BranchCode) : "",
                                        detailCheck.CheckStatus == "ReferToOfficer" ? _commonClass.GetUserDisplayName(detailCheck.ClearingOfficer.GetValue()) : ""),

                        Reason = string.Format("{0}", detailCheck.ReasonDesc),//string.Format("{0}", detailCheck.CheckStatus == "Reject" || detailCheck.CheckStatus == "ReAssign" || detailCheck.CheckStatus == "ReferToOfficer" ? _commonClass.GetRejectReasonDesc(detailCheck.Reason): ""),
                        VerifiedBy = detailCheck.CheckStatus == "Open" ? "" : detailCheck.VerifiedBy,
                        VerifiedDateTime = detailCheck.CheckStatus == "Open" ? "" : detailCheck.VerifiedDateTime.GetDateValue(),
                        ApprovedBy = detailCheck.CheckStatus == "Open" ? "" : detailCheck.ApprovedBy,
                        ApprovedDateTime = detailCheck.CheckStatus == "Open" ? "" : detailCheck.ApprovedDateTime.GetDateValue(),
                        ChequeImageFileContentType = await GetChequeImageFileContentType(detailCheck.ChequeImageLinkedKey),
                        ChequeImageFileContent = await GetChequeImageFileContent(detailCheck.ChequeImageLinkedKey),
                        ChequeImageFileContentR = await GetChequeImageFileContentR(detailCheck.ChequeImageLinkedKey),
                        ChequeImageFileContentF = await GetChequeImageFileContentF(detailCheck.ChequeImageLinkedKey),
                        ChequeStats = _commonClass.GetDetailsFromHistory(detailCheck.ChequeImageLinkedKey)
                    };

                    status.StatusCode = "00";
                    status.Data = checkDetailModel;
                }
            }
            catch (Exception ex)
            {
                status.StatusCode = "01";
                status.StatusMessage = ex.Message;
            }
            return status;
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintReport(string Key, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null)
        {
            ReturnGenericData<ReturnDownloadPDF> status = new ReturnGenericData<ReturnDownloadPDF>();
            try
            {
                IQueryable<InwardClearingChequeDetailsModel> res = await QueryableReportList(Key, dateFromFilter, dateToFilter, BSRTNFilter);
                List<InwardClearingChequeDetailsModel> chekReportDetailList = res.Take(20).ToList<InwardClearingChequeDetailsModel>();
                List<InwardClearingReportModel> inwardClearingReportList = await FGetReportList(chekReportDetailList);

                string effectiveDate = "", sAcctNo = "",
                        sTotalItems = "",
                        sTotalAmount = "",
                        sTmpAmt = "0";
                if (!string.IsNullOrEmpty(Key))
                    Key = char.ToUpper(Key[0]) + Key.Substring(1);
                string reportFilenNme = Key.IsNull() ? "" :
                       Key.StartsWith("LargeAmount") ? "LargeAmount" : Key.StartsWith("WithTechnicalities") ? "WithTechnicalities" : Key.StartsWith("MCImages") ? "MCImages" : Key;

                string filename = string.Format("{0}Report{1}.pdf", reportFilenNme, DateTime.Now.GetDateValue()).Replace("/", "").Replace(":", "").Replace(" ", "");
                foreach (InwardClearingChequeDetailsModel item in chekReportDetailList)
                {
                    if (Key == "Upload")
                    {
                        if (effectiveDate != item.EffectivityDate.ToDate().GetDateValue() && effectiveDate != "")
                        {
                            inwardClearingReportList.Add(
                                new InwardClearingReportModel
                                {
                                    BranchCode = "Total",
                                    EffectivityDate = effectiveDate,
                                    TotalItems = sTotalItems,
                                    TotalAmount = sTotalAmount
                                }
                                );

                            effectiveDate = item.EffectivityDate.ToDate().GetDateValue();
                            sTotalItems = "1";
                            sTotalAmount = item.CheckAmount.GetFloatValue();
                        }
                        else
                        {
                            sTotalItems = (sTotalItems.ToInt() + (item.TotalItems == null ? "1" : item.TotalItems).ToInt()).ToString();
                            sTmpAmt = (item.TotalAmount == null ? item.CheckAmount.GetFloatValue() : item.TotalAmount.GetFloatValue());
                            sTotalAmount = (sTotalAmount.ToDouble(0) + sTmpAmt.ToDouble(0)).ToString();
                            effectiveDate = item.EffectivityDate.ToDate().GetDateValue();
                        }

                    }
                    else
                    {
                        if (Key.StartsWith("Acct"))
                        {
                            sTotalItems = (sTotalItems.ToInt() + (item.TotalItems == null ? "1" : item.TotalItems).ToInt()).ToString();
                            sTmpAmt = (item.TotalAmount == null ? item.CheckAmount.GetFloatValue() : item.TotalAmount.GetFloatValue());
                            sTotalAmount = (sTotalAmount.ToDouble(0) + sTmpAmt.ToDouble(0)).ToString();
                            effectiveDate = item.EffectivityDate.ToDate().GetDateValue();
                        }
                        else
                        {
                            if (sAcctNo != item.AccountNumber.Substring(0, 3) && sAcctNo != "")
                            {
                                inwardClearingReportList.Add(
                                    new InwardClearingReportModel
                                    {
                                        BranchCode = "Total",
                                        EffectivityDate = effectiveDate,
                                        TotalItems = sTotalItems,
                                        TotalAmount = sTotalAmount
                                    }
                                    );

                                effectiveDate = item.EffectivityDate.ToDate().GetDateValue();
                                sTotalItems = "1";
                                sTotalAmount = item.CheckAmount.GetFloatValue();
                                sAcctNo = item.AccountNumber.Substring(0, 3);
                            }
                            else
                            {
                                sTotalItems = (sTotalItems.ToInt() + (item.TotalItems == null ? "1" : item.TotalItems).ToInt()).ToString();
                                sTmpAmt = (item.TotalAmount == null ? item.CheckAmount.GetFloatValue() : item.TotalAmount.GetFloatValue());
                                sTotalAmount = (sTotalAmount.ToDouble(0) + sTmpAmt.ToDouble(0)).ToString();
                                effectiveDate = item.EffectivityDate.ToDate().GetDateValue();
                                sAcctNo = item.AccountNumber.Substring(0, 3);
                            }
                        }
                    }
                }
                inwardClearingReportList.Add(
                new InwardClearingReportModel
                {
                    BranchCode = "Total",
                    EffectivityDate = effectiveDate,
                    TotalItems = sTotalItems,
                    TotalAmount = sTotalAmount
                });
                string sTextListOpen = "Account Number|Check Number|Check Amount|Uploaded By|Uploaded Date|Effectivity Date",
                        sTextListReAssign = "Account Number|Check Number|Check Amount|Verified By|Verified Date|Reassigned By|ReAssign Reason|Branch Code",
                        sTextListAccept = "Account Number|Check Number|Check Amount|Verified By|Verified Date|Reassigned By|ReAssign Reason|Approved By|Approved Date",
                        sTextListReject = "Account Number|Check Number|Check Amount|Verified By|Verified Date|Reassigned By|ReAssign Reason|Reject By|Reject Reason|Reject Date",
                        sTextListReferToOfficer = "Account Number|Check Number|Check Amount|Verified By|Verified Date|Next Level Approver",
                        sTextListUpload = "Branch Code|Upload Date|Total Items|Total Amount",
                        sTextListAcctStat = "Account Number|Check Number|Check Amount";
                int i = 0;
                Dictionary<int, List<string>> DataList = inwardClearingReportList
                                                      .ToDictionary(x => i++, x =>
                                                     (
                                (Key == "Upload") ?
                                    (x.BranchCode == "Total") ?
                                        string.Format("{0}|{1}|{2}",
                                                    x.BranchCode,
                                                    x.TotalItems.ToInt().ToString("#,##0.00"),
                                                    x.TotalAmount.ToDouble(0).ToString("#,##0.00"))
                                    :
                                        string.Format("{0}|{1}|{2}|{3}",
                                                x.BranchCode,
                                                x.EffectivityDate,
                                                x.TotalItems,
                                                x.TotalAmount)
                                :
                                (Key.StartsWith("Acct")) ?
                                    (x.BranchCode == "Total") ?
                                        string.Format("{0}|{1}|{2}",
                                                    x.BranchCode,
                                                    x.TotalItems.ToInt().ToString("#,##0.00"),
                                                    x.TotalAmount.ToDouble(0).ToString("#,##0.00"))
                                    :
                                        string.Format("{0}|{1}|{2}",
                                                x.AccountNumber,
                                                x.CheckNumber,
                                                x.CheckAmount.ToString("#,##0.00"))
                                :
                                    (x.BranchCode == "Total") ?
                                        string.Format("{0}|{1}|{2}",
                                                    x.BranchCode,
                                                    x.TotalItems.ToInt().ToString("#,##0.00"),
                                                    x.TotalAmount.ToDouble(0).ToString("#,##0.00"))
                                    :
                                    string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                                                x.AccountNumber,
                                                x.CheckNumber,
                                                x.CheckAmount.ToString("#,##0.00") + "   ",
                                                (Key == "ReAssign") ? x.ClearingOfficer : x.VerifiedBy,
                                                x.VerifiedDateTime,
                                                (Key == "Open") ? x.EffectivityDate :
                                                (Key == "ReAssign") ? string.Format("{0}|{1}|{2}", x.ReassignedBy, x.ReAssignReason, x.BranchCode) :
                                                (Key == "Accept") ? string.Format("{0}|{1}|{2}|{3}", x.ReassignedBy, x.ReAssignReason, x.ApprovedBy, x.ApprovedDateTime) :
                                                (Key == "Reject") ? string.Format("{0}|{1}|{2}|{3}|{4}", x.ReassignedBy, x.ReAssignReason, x.ApprovedBy, x.RejectReason, x.ApprovedDateTime) :
                                                (Key.StartsWith("Acct")) ? string.Format("{0}|{1}|{2}|{3}|{4}", x.ReassignedBy, x.ReAssignReason, x.ApprovedBy, x.RejectReason, x.ApprovedDateTime) :
                                                (Key == "ReferToOfficer") ? x.ClearingOfficer : ""
                                                )
                            ).Split("|").ToList());

                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userName = userClaims.DisplayName;

                string title = Key.StartsWith("Acct") ? (Key == "ReferToOfficer" ? "Next Level Approver" : Key.Split("^")[0]) : _commonClass.GetStatusDesc(Key);
                PDFReport pDFReport = new PDFReport();
                string TitleHeader = string.Format("Signature Verification {0} Report", title);
                List<int> widthList = new List<int> { 90, 80, 100, 90, 100, 90, 130, 90, 130, 100 };
                List<string> Header = (
                                    (Key == "Open") ? sTextListOpen :
                                    (Key == "ReAssign") ? sTextListReAssign :
                                    (Key == "Accept") ? sTextListAccept :
                                    (Key == "Reject") ? sTextListReject :
                                    (Key == "ReferToOfficer") ? sTextListReferToOfficer :
                                    (Key == "Upload") ? sTextListUpload :
                                    (Key.StartsWith("Acct")) ? sTextListAcctStat : sTextListOpen
                                ).Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                byte[] data = pDFReport.GeneratePDF(
                                          DataList,
                                          title, userName, userId,
                                          TitleHeader,
                                          Header,
                                          widthList
                                          );
                string base64String = Convert.ToBase64String(data);

                var printReportResult = new ReturnDownloadPDF
                {
                    PdfDataBase64 = base64String,
                    FileName = filename
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
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintReportWithImages(string Key, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null)
        {
            ReturnGenericData<ReturnDownloadPDF> status = new ReturnGenericData<ReturnDownloadPDF>();
            try
            {
                IQueryable<InwardClearingChequeDetailsModel> res = await QueryableReportList(Key, dateFromFilter, dateToFilter, BSRTNFilter);
                List<InwardClearingChequeDetailsModel> chekReportDetailList = res.ToList<InwardClearingChequeDetailsModel>();

                var reportFilenNme = Key.IsNull() ? "" :
                      Key.StartsWith("LargeAmount") ? "LargeAmount" : Key.StartsWith("WithTechnicalities") ? "WithTechnicalities" : Key.StartsWith("MCImages") ? "MCImages" : Key;

                string filename = string.Format("{0}Report{1}.pdf", reportFilenNme, DateTime.Now.GetDateValue()).Replace("/", "").Replace(":", "").Replace(" ", "");

                string ImageFolder = _commonClass.GetImageFolder();
                string sTmpBranch = "",
                        sTotalItems = "0",
                        sTotalAmount = "0",
                        sTmpAmt = "0";
                List<InwardClearingReportModel> inwardClearingReportList = new List<InwardClearingReportModel>();
                foreach (InwardClearingChequeDetailsModel item in chekReportDetailList)
                {
                    if (sTmpBranch != item.Brstn && sTmpBranch != "")
                    {
                        inwardClearingReportList.Find(x => x.BranchBRSTN == sTmpBranch).TotalItems = sTotalItems.ToInt().ToString("#,##0");
                        inwardClearingReportList.Find(x => x.BranchBRSTN == sTmpBranch).TotalAmount = sTotalAmount.ToDouble(0).ToString("#,##0.00");

                        sTmpBranch = item.Brstn;
                        sTotalItems = "1";
                        sTotalAmount = "0";
                    }
                    else
                    {
                        sTotalItems = (sTotalItems.ToInt() + (item.TotalItems == null ? "1" : item.TotalItems).ToInt()).ToString();
                        sTmpAmt = (item.TotalAmount == null ? item.CheckAmount.GetFloatValue() : item.TotalAmount.GetFloatValue());
                        sTotalAmount = (sTotalAmount.ToDouble(0) + sTmpAmt.ToDouble(0)).ToString("#,##0.00");
                        sTmpBranch = item.Brstn;
                    }

                    string sLinkedKey = string.Format("{0}_{1}_{2}_{3}",
                                                item.EffectivityDate.ToDate("MMddyyyy"),
                                                item.AccountNumber,
                                                item.CheckNumber,
                                                item.CheckAmount.GetFloatValue()),
                           FileContent = await GetChequeImageFileContent(sLinkedKey),
                           FileContentR = await GetChequeImageFileContentR(sLinkedKey, true),
                           FileContentF = await GetChequeImageFileContentF(sLinkedKey, true);



                    string tmpDetHistory = GetDetailsFromHistory(item.ChequeImageLinkedKey.GetValue(), "ReAssign").GetAwaiter().GetResult();
                    string tmpReAssignReason = GetReasonFromHistory(item.ChequeImageLinkedKey.GetValue(), "ReAssign").GetAwaiter().GetResult();
                    string[] tmpUserDet = tmpDetHistory.Split('|');

                    inwardClearingReportList.Add(
                                    new InwardClearingReportModel
                                    {
                                        AccountNumber = item.AccountNumber.GetValue(),
                                        CheckNumber = item.CheckNumber.GetValue(),
                                        CheckAmount = item.CheckAmount,
                                        EffectivityDate = item.EffectivityDate.GetDateValue(),
                                        CheckStatus = _commonClass.GetStatusDesc(item.CheckStatus), //item.CheckStatus.GetValue(),
                                        RejectReason = GetReasonFromHistory(item.ChequeImageLinkedKey.GetValue(), "Reject").GetAwaiter().GetResult(),
                                        ReAssignReason = tmpReAssignReason, //GetReasonFromHistory(item.ChequeImageLinkedKey.GetValue(), "ReAssign"),
                                        VerifiedBy = (tmpReAssignReason != "") ? tmpUserDet[0] : item.VerifiedBy.GetValue(),
                                        VerifiedDateTime = (tmpReAssignReason != "") ? tmpUserDet[1] : item.VerifiedDateTime.ToString(), // .GetDateValue(),
                                        ApprovedBy = item.ApprovedBy.GetValue(),
                                        ApprovedDateTime = item.ApprovedDateTime.ToString(), //.GetDateValue(),
                                        BranchCode = item.BranchCode.GetValue(),
                                        ClearingOfficer = item.ClearingOfficer.GetValue(),
                                        TotalItems = item.TotalItems,
                                        TotalAmount = item.TotalAmount,
                                        FrontImage = FileContentF == "" ? FileContent : FileContentF,
                                        BackImage = FileContentR,
                                        BranchBRSTN = item.Brstn.GetValue(),
                                        BranchName = _commonClass.GetBranchOfAssignmentDesc(item.BranchCode.GetValue()),
                                        ChequeStats = _commonClass.GetDetailsFromHistory(item.ChequeImageLinkedKey.GetValue())
                                    }
                                );
                }
                if (sTmpBranch != "")
                {
                    inwardClearingReportList.Find(x => x.BranchBRSTN == sTmpBranch).TotalItems = sTotalItems.ToInt().ToString("#,##0");
                    inwardClearingReportList.Find(x => x.BranchBRSTN == sTmpBranch).TotalAmount = sTotalAmount.ToDouble(0).ToString("#,##0.00");
                }

                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userName = userClaims.DisplayName;

                string title = Key.StartsWith("Acct") ? (Key == "ReferToOfficer" ? "Next Level Approver" : Key.Split("^")[0]) : _commonClass.GetStatusDesc(Key);
                PDFReport pDFReport = new PDFReport();
                string TitleHeader = string.Format("Signature Verification {0} Report", title);
                List<int> widthList = new List<int> { 90, 80, 100, 90, 100, 90, 130, 90, 130, 100 };
                List<string> Header = [];
                byte[] data = pDFReport.GeneratePDFWithImage(
                                Key, dateFromFilter, dateToFilter, BSRTNFilter,
                                ImageFolder,
                                inwardClearingReportList,
                                string.Format("{0} Report", Key), userName, userId);
                string base64String = Convert.ToBase64String(data);

                var printReportResult = new ReturnDownloadPDF
                {
                    PdfDataBase64 = base64String,
                    FileName = filename
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

        #region Function
        public async Task<IQueryable<InwardClearingChequeDetailsModel>> QueryableReportList(string Key, string? dateFromFilter = null, string? dateToFilter = null, string? BSRTNFilter = null)
        {
            try
            {
                var chekReportDetail = _dBContext.InwardClearingChequeDetailsModels;
                DateTime dateFrom = dateFromFilter.ToDate().IsValidDate() ? dateFromFilter.ToDate() : DateTime.Now.Date,
                       dateTo = dateToFilter.ToDate().IsValidDate() ? dateToFilter.ToDate() : DateTime.Now.Date;

                string status = ""; 
                if (!string.IsNullOrEmpty(Key))
                    Key = char.ToUpper(Key[0]) + Key.Substring(1);
                //string userType = User.Claims.SingleOrDefault(e => e.Type == "UserType").Value;
                //string userBranchBRSTN = _commonClass.GetBranchBRSTN(User.Claims.SingleOrDefault(e => e.Type == "BranchCode").Value);

                string userType = "";
                string userBranchBRSTN = "";
                if (userType.Contains("Branch"))
                    BSRTNFilter = BSRTNFilter ?? userBranchBRSTN;
                string brstn = BSRTNFilter;

                IQueryable<InwardClearingChequeDetailsModel> res = chekReportDetail.OrderBy(e => e.AccountNumber);
                List<InwardClearingChequeDetailsModel> chekReportDetailList = new List<InwardClearingChequeDetailsModel>();

                if (Key != "")
                {
                    if (Key == "Open")
                        res = chekReportDetail.Where(e => e.CheckStatus == Key && e.EffectivityDate.Date >= dateFrom && e.EffectivityDate.Date <= dateTo);

                    if (Key == "ReferToOfficer" || Key == "ReAssign")
                        res = chekReportDetail.Where(e => e.CheckStatus == Key && e.EffectivityDate.Date >= dateFrom && e.EffectivityDate.Date <= dateTo);

                    if (Key == "Reject" || Key == "Accept")
                        res = chekReportDetail.Where(e => e.CheckStatus == Key && e.EffectivityDate.Date >= dateFrom && e.EffectivityDate.Date <= dateTo);

                    if (Key == "Upload")
                        res = chekReportDetail
                            .Where(w => w.EffectivityDate.Date >= dateFrom && w.EffectivityDate.Date <= dateTo)
                            .GroupBy(g => new { g.BranchCode })
                            .Select(s => new InwardClearingChequeDetailsModel
                            {
                                BranchCode = s.Key.BranchCode,
                                TotalItems = s.Count().ToString(),
                                TotalAmount = s.Sum(c => c.CheckAmount).ToString("#,##0.00")
                            }
                            );

                    if (Key.StartsWith("LargeAmount"))
                    {
                        double dCheckAmount = Key.Contains("^") ? Key.Split('^')[1].ToDouble(0) : 0;
                        res = chekReportDetail.Where(e => e.CheckAmount > dCheckAmount
                                && e.EffectivityDate.Date >= dateFrom
                                && e.EffectivityDate.Date <= dateTo);

                        if (!brstn.IsNull())
                            res = res.Where(e => e.Brstn.Trim() == brstn.Trim());
                    }

                    if (Key.Equals("WithTechnicalities"))
                    {
                        res = chekReportDetail.Where(e => (e.Reason == null) == false
                                && e.EffectivityDate.Date >= dateFrom
                                && e.EffectivityDate.Date <= dateTo);

                        if (!brstn.IsNull())
                            res = res.Where(e => e.Brstn.Trim() == brstn.Trim());
                    }

                    if (Key.Equals("MCImages"))
                    {
                        res = chekReportDetail.Where(e => e.AccountNumber.Substring(0, 3).Equals("000")
                                            && e.EffectivityDate.Date >= dateFrom
                                            && e.EffectivityDate.Date <= dateTo)
                                            .OrderBy(e => e.Brstn)
                                            .OrderBy(e => e.AccountNumber);

                        if (!brstn.IsNull())
                            res = res.Where(e => e.Brstn.Trim() == brstn.Trim()).OrderByDescending(e => e.CheckAmount);
                    }

                    //account stat
                    if (Key.StartsWith("Acct"))
                    {
                        if (Key.Contains("Clsd"))
                        { status = "CLOSED"; }

                        if (Key.Contains("Drmt"))
                        { status = "DORMANT"; }

                        if (Key.Contains("PnD"))
                        { status = "Post No Debit"; }

                        res = GetInwardClearingReports(status, dateFrom, dateTo);

                        if (!brstn.IsNull())
                            res = res.Where(e => e.Brstn.Trim() == brstn.Trim());
                    }

                    if (Key == "Upload")
                        res = res.OrderBy(e => e.BranchCode);
                    else if (Key.StartsWith("LargeAmount"))
                        res = res.OrderBy(e => e.CheckAmount).OrderBy(e => e.BranchCode);
                    else if (Key.StartsWith("WithTechnicalities"))
                        res = res.OrderBy(e => e.Brstn);
                    else if (!Key.StartsWith("Acct"))
                        res = res.OrderBy(e => e.EffectivityDate).OrderBy(e => e.AccountNumber);

                }
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<List<InwardClearingReportModel>> FGetReportList(List<InwardClearingChequeDetailsModel> chekReportDetailList)
        {
            try
            {
                List<InwardClearingReportModel> inwardClearingReportList = new List<InwardClearingReportModel>();
                Dictionary<ChequeStats, string> detailsFromHistory = new Dictionary<ChequeStats, string>();

                foreach (InwardClearingChequeDetailsModel item in chekReportDetailList)
                {
                    string[] reassign = GetDetailsFromHistory(item.ChequeImageLinkedKey.GetValue(), "ReAssign").GetAwaiter().GetResult().Split('|');

                    inwardClearingReportList.Add(
                                    new InwardClearingReportModel
                                    {
                                        AccountNumber = item.AccountNumber.GetValue(),
                                        CheckNumber = item.CheckNumber.GetValue(),
                                        CheckAmount = item.CheckAmount,
                                        EffectivityDate = item.EffectivityDate.GetDateValue(),
                                        CheckStatus = _commonClass.GetStatusDesc(item.CheckStatus),//item.CheckStatus.GetValue(), 
                                        RejectReason = GetReasonFromHistory(item.ChequeImageLinkedKey.GetValue(), "Reject").GetAwaiter().GetResult(),
                                        ReAssignReason = GetReasonFromHistory(item.ChequeImageLinkedKey.GetValue(), "ReAssign").GetAwaiter().GetResult(),
                                        VerifiedBy = item.VerifiedBy.GetValue(),
                                        VerifiedDateTime = item.VerifiedDateTime.GetDateValue(),
                                        ApprovedBy = item.ApprovedBy.GetValue(),
                                        ApprovedDateTime = item.ApprovedDateTime.GetDateValue(),
                                        ReassignedBy = reassign[0],
                                        ReassignedDateTime = reassign[1],
                                        BranchCode = item.BranchCode.GetValue(),
                                        ClearingOfficer = item.ClearingOfficer ?? item.VerifiedBy.GetValue(),
                                        TotalItems = item.TotalItems,
                                        TotalAmount = item.TotalAmount,
                                        BranchBRSTN = item.Brstn.GetValue().Trim(),
                                        ChequeStats = _commonClass.GetDetailsFromHistory(item.ChequeImageLinkedKey.GetValue())
                                    }
                                );
                }
                return inwardClearingReportList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private IQueryable<InwardClearingChequeDetailsModel> GetInwardClearingReports(string status, DateTime dateFrom, DateTime dateTo)
        {
            IQueryable<InwardClearingChequeDetailsModel> inwardClearingReports =
                 from t1 in _dBContext.InwardClearingChequeDetailsModels.Where(e => e.EffectivityDate.Date >= dateFrom.Date && e.EffectivityDate.Date <= dateTo.Date)
                 join t2 in _dBContext.ChequeAccountDetails on t1.ChequeImageLinkedKey equals t2.ChequeImageLinkedKey
                 where t2.AccountStatus.Contains(status) && t2.EffectivityDate.Value.Date >= dateFrom.Date && t2.EffectivityDate.Value.Date <= dateTo.Date
                 orderby t2.StatusAsOfDate, t1.AccountNumber
                 select new InwardClearingChequeDetailsModel
                 {
                     AccountNumber = t1.AccountNumber,
                     CheckNumber = t1.CheckNumber,
                     CheckAmount = t1.CheckAmount,
                     EffectivityDate = t1.EffectivityDate,
                     CheckStatus = t1.CheckStatus,
                     VerifiedBy = t1.VerifiedBy,
                     VerifiedDateTime = t1.VerifiedDateTime,
                     ApprovedBy = t1.ApprovedBy.GetValue(),
                     ApprovedDateTime = t1.ApprovedDateTime,
                     BranchCode = t1.BranchCode.GetValue(),
                     ClearingOfficer = t1.ClearingOfficer ?? t1.VerifiedBy.GetValue(),
                     TotalItems = t1.TotalItems,
                     TotalAmount = t1.TotalAmount,
                     Brstn = t1.Brstn.GetValue().Trim()
                 };
            return inwardClearingReports;
        }
        private async Task<string> GetReasonFromHistory(string chequeImageLinkedKey, string checkStatus)
        {
            InwardClearingChequeHistoryModel? result = await _dBContext.InwardClearingChequeHistoryModels.Where(
                                                      e => e.ChequeImageLinkedKey == chequeImageLinkedKey && e.CheckStatusTo == checkStatus).OrderBy(x => x.Id).LastOrDefaultAsync();

            if (result != null)
                return _commonClass.GetRejectReasonDesc(result.Reason ?? "");

            return "";
        }
        private async Task<string> GetDetailsFromHistory(string chequeImageLinkedKey, string checkStatus)
        {
            InwardClearingChequeHistoryModel? result =
                await _dBContext.InwardClearingChequeHistoryModels.Where(
                    e => e.ChequeImageLinkedKey == chequeImageLinkedKey && e.CheckStatusTo == checkStatus).OrderBy(x => x.Id).LastOrDefaultAsync();
            if (result != null)
                return string.Format("{0}|{1}", result.ActionBy, result.ActionDateTime);

            return "|";
        }
        private async Task<string> GetChequeImageFileContent(string chequeImageLinkedKey)
        {
            InwardClearingChequeImageModel result = await _dBContext.InwardClearingChequeImageModels
                .Where(e => e.ChequeImageLinkedKey == chequeImageLinkedKey)
                .OrderBy(e => e.Id)
                .LastOrDefaultAsync();
            if (!result.IsNull())
                return result.ChequeImageFileContent;

            return "";
        }
        private async Task<string> GetChequeImageFileContentF(string chequeImageLinkedKey, bool forReport = false)
        {
            try
            {
                string[] fileSuffixFront = _commonClass.GetPrefixFront();
                return await GetChequeFile(chequeImageLinkedKey, fileSuffixFront, forReport);
            }
            catch
            {
                throw;
            }
        }
        private async Task<string> GetChequeImageFileContentR(string chequeImageLinkedKey, bool forReport = false)
        {
            try
            {
                string[] fileSuffixBack = _commonClass.GetPrefixBack();
                return await GetChequeFile(chequeImageLinkedKey, fileSuffixBack, forReport);
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> GetChequeFile(string chequeImageLinkedKey,string[] fileSuffixFront, bool forReport)
        {
            try
            {
                InwardClearingChequeImageModel result = null;
                foreach (var suffix in fileSuffixFront)
                {
                    result = await _dBContext.InwardClearingChequeImageModels
                        .Where(e => e.ChequeImageLinkedKey == string.Format("{0}_{1}", chequeImageLinkedKey, suffix))
                        .OrderBy(e => e.Id)
                        .LastOrDefaultAsync();

                    if (!result.IsNull())
                        break;
                }
                string r = string.Empty;
                if (!result.IsNull())
                {
                    if ((result.ChequeImageFileContent.IsBase64String()))
                    {
                        if (forReport)
                            r = result.ChequeImageFileContent.FromBase64String().ByteArrayToPngBase64String();
                        else
                            r = string.Format("data:{0}; base64 , {1}", result.ChequeImageFileContentType, result.ChequeImageFileContent.FromBase64String().ByteArrayToPngBase64String());
                    }
                    else
                    {
                        r = string.Format(_commonClass.GetImageFolder() + "{0}\\{1}", result.ChequeImageFileContent.Replace('|', '/'), result.ChequeImageLinkedKey);
                        byte[] b = _commonClass.ReadFileAsBytes(r);
                        if (!forReport)
                            r = string.Format("data:{0}; base64 , {1}", result.ChequeImageFileContentType, b.ByteArrayToPngBase64String());
                        else
                            r = b.ByteArrayToPngBase64String();
                    }
                }
                else
                {
                    r = GetNoChequeImage();
                    if (!forReport)
                        string.Format("data: image/jpeg; base64 , {0}", r);
                }
                return r;
            }
            catch
            {
                throw;
            }
        }
        private async Task<string> GetChequeImageFileContentType(string chequeImageLinkedKey)
        {
            try
            {
                InwardClearingChequeImageModel result = await _dBContext.InwardClearingChequeImageModels
                    .Where(e => e.ChequeImageLinkedKey == chequeImageLinkedKey)
                    .OrderBy(e => e.Id)
                    .LastOrDefaultAsync();
                if (!result.IsNull())
                    return result.ChequeImageFileContentType;
            }
            catch { }
            return "";
        }
        private string GetNoChequeImage() { return "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"; }//"noimages.png".ReadResource(true); }
        #endregion
    }
}
