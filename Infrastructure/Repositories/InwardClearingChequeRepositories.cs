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
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class InwardClearingChequeRepositories : IInwardClearingChequeRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly IConfiguration _config;
        private readonly PRDSVSContext _contextSVS;
        private readonly GetUserClaims _userClaims;
        private readonly FinacleSVSContext _contextFinacleSVS;

        public InwardClearingChequeRepositories(AppDbContext dBContext, 
            ADClass addClass, 
            CommonClass commonClass, 
            IConfiguration config,
            PRDSVSContext contextSVS,
            IHttpContextAccessor httpContextAccessor,
            FinacleSVSContext contextFinacleSVS)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _contextSVS = contextSVS;
            _config = config;
            _contextFinacleSVS = contextFinacleSVS;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }


        public ReturnGenericDropdown GetStatusList()
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            try
            {
                List<SelectListItem> result = new List<SelectListItem> { };

                string[] texts = new string[] { "ALL:ALL", "Open:OP" };

                foreach (string items in texts)
                {
                    string[] item = items.Split(':');
                    result.Add(new SelectListItem { Text = item[0], Value = item[1], IsChecked = false });
                }
                status = _commonClass.MsgSuccess("");
                x.Data = result;
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            x.StatusMessage = status.StatusMessage;
            x.StatusCode = status.StatusCode;
            return x;
        }
        public async Task<ReturnGenericData<PaginatedOutput<InwardClearingChequeDetailsModel>>> GetICCList(string? key, int? page, ParamInwardClearingChequeGetList value)
        {
            var obj = new ReturnGenericData<PaginatedOutput<InwardClearingChequeDetailsModel>>();
            obj.StatusCode = "01";
            try
            {
                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userType = userClaims.UserType;
                string userBranch = userClaims.BranchCode;
                string userBranchBRSTN = _commonClass.GetBranchBRSTN(userBranch);
             

                if (userType.Contains("Branch"))
                    value.BSRTNFilter = value.BSRTNFilter ?? userBranchBRSTN;

                
                string logMessage = "", _sendEmailTo = "", sBranchCode = "", sReason = "";
                
                DateTime dateFrom = value.DateFromFilter.IsValidDate() ? value.DateFromFilter.Date : DateTime.Now.Date,
                dateTo = value.DateToFilter.IsValidDate() ? value.DateToFilter.Date : DateTime.Now.Date;

                var inwardClearingChequeDetails = GetInwardClearingChequeDetailsModelFiltered(key ?? value.CheckImageFilter ?? "ALL", value.AccountNumberFilter ?? "", value.BSRTNFilter ?? "", value.AmountFromFilter ?? "", value.AmountToFilter ?? "", dateFrom, dateTo);
                var filters = "";//accountNumberFilter + '|' + sBSRTNFilter + '|' + amountFromFilter + '|' + amountToFilter
                var sCheckStatus = (key == "RB") ? "ReAssign" : (key == "RO") ? "ReferToOfficer" : "Open";
                PaginatedList<InwardClearingChequeDetailsModel> inwardClearingChequeDetailsPaged = PaginatedList<InwardClearingChequeDetailsModel>.Create(inwardClearingChequeDetails, page ?? 1);

                foreach (InwardClearingChequeDetailsModel items in inwardClearingChequeDetailsPaged)
                {
                    if (items.ChequeImageLinkedKey != items.GenChequeImageLinkedKey)
                        UpdateChequeImageLinkedKey(items.Id);

                    if (items.CheckStatus == sCheckStatus)
                    {
                        filters = string.Format("{0}|{1}|{2}|{3}", value.AccountNumberFilter.GetValue(), value.BSRTNFilter.GetValue(), value.AmountFromFilter.GetValue(), value.AmountToFilter.GetValue());
                        items.NextSelectedCheck = _commonClass.GetNextChequeImageLinkedKey(items.ChequeImageLinkedKey, sCheckStatus, filters, dateFrom.GetDateValue(), dateTo.GetDateValue(), items.BranchCode, items.ClearingOfficer);
                    }
                }

                if (key == "RB" || userType.Contains("Branch"))
                {
                    string[] branchBuddy = _commonClass.GetBuddyBranches(userBranch);

                    List<string> branches = new List<string>();
                    branches.Add(userBranch);
                    branches.AddRange(branchBuddy);

                    //MakeViewBagBranchListByCode(branches.ToArray(), checkViewModel.BSRTNFilter);
                }
                var data = new PaginatedOutput<InwardClearingChequeDetailsModel>(inwardClearingChequeDetailsPaged);
                foreach (var model in data.Data)
                {
                    model.ReasonDesc = _commonClass.GetRejectReasonDesc(model.Reason);
                    model.CheckStatusDisplay = _commonClass.GetStatusDesc(model.CheckStatus);
                    model.NextSelectedCheck = _commonClass.GetNextChequeImageLinkedKey(model.ChequeImageLinkedKey, model.CheckStatus, "", model.EffectivityDate.ToString(), model.EffectivityDate.ToString(), model.BranchCode, model.ClearingOfficer);
                }

                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
                obj.Data = data;
            }
            catch (Exception err)
            {
                obj.StatusMessage = err.Message;
            }
            return obj;
        }
        public async Task<ReturnGenericStatus> SubmitCheck(ParamSaveChequeDetails value)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userType = userClaims.UserType;
                string userBranch = userClaims.BranchCode;
                string userBranchBRSTN = _commonClass.GetBranchBRSTN(userBranch);

                string userBranchBuddy = userClaims.BuddyCode;
                string userBranchBuddyName = userClaims.BuddyBranch;
                string userBranchBuddyBRSTN = _commonClass.GetBranchBRSTN(userBranchBuddy);

                string logMessage = "", _sendEmailTo = "", sBranchCode = "", sReason = "";
                if (!string.IsNullOrEmpty(value.VerifyAction))
                {
                    var updateCheck = _dBContext.InwardClearingChequeDetailsModels.Find(GetInwardClearingChequeDetailsID(value.SelectedCheck));

                    if (!updateCheck.IsNull())
                    {
                        string sSelecteCheck = string.Format("Date: {0} <br> Account: {1} <br> Check No: {2} <br> Amount: {3} <br>", value.SelectedCheck.Split('_'));
                        if (AllowedStatusUpdate(updateCheck.CheckStatus, value.VerifyAction))
                        {
                            if (updateCheck.CheckStatus == "Open")
                            {
                                updateCheck.VerifiedBy = userId;
                                updateCheck.VerifiedDateTime = DateTime.Now;
                                updateCheck.ApprovedBy = userId;
                                updateCheck.ApprovedDateTime = DateTime.Now;
                            }

                            if (value.VerifyAction == "Reject")
                            {
                                updateCheck.Reason = value.SelectedReasonCode;
                                sBranchCode = updateCheck.BranchCode;
                                sReason = _commonClass.GetRejectReasonDesc(value.SelectedReasonCode);

                                _sendEmailTo = "CLR";

                                string sBranchDesc = _commonClass.GetBranchOfAssignmentDesc(sBranchCode);
                                logMessage = string.Format("User : {0} Branch : {1} Reject/Return.  Reason: {2} Check Details: {3}",
                                                            userId, sBranchDesc, sReason, sSelecteCheck);
                            }

                            if (value.VerifyAction == "ReAssign")
                            {
                                updateCheck.Reason = value.ReassignReason;
                                sBranchCode = updateCheck.BranchCode;
                                sReason = _commonClass.GetRejectReasonDesc(value.ReassignReason);

                                _sendEmailTo = "BRN";

                                string sBranchDesc = _commonClass.GetBranchOfAssignmentDesc(sBranchCode);
                                logMessage = string.Format("User : {0} ReAssign to Branch : {1} Reason: {2} Check Details: {3}",
                                                            userId, sBranchDesc, sReason, sSelecteCheck);
                            }

                            if (value.VerifyAction == "ReferToOfficer")
                            {
                                updateCheck.ClearingOfficer = value.SelectedOfficerCode;
                                logMessage = string.Format("User : {0} ReferToOfficer : {1} Check Details: {2}",
                                                            userId, value.SelectedOfficerCode, sSelecteCheck);
                            }

                            if (value.VerifyAction == "Accept" || value.VerifyAction == "Reject")
                            {
                                if (updateCheck.CheckStatus == "ReAssign" || updateCheck.CheckStatus == "ReferToOfficer")
                                {
                                    updateCheck.ApprovedBy = userId;
                                    updateCheck.ApprovedDateTime = DateTime.Now;

                                    logMessage = string.Format("User : {0} Action : {1} = {2} check. Check Details: {3}",
                                                                userId, value.VerifyAction, updateCheck.CheckStatus, sSelecteCheck);
                                }
                            }

                            string[] sParams = new string[]
                                {
                                updateCheck.ChequeImageLinkedKey,
                                updateCheck.CheckStatus,
                                value.VerifyAction,
                                updateCheck.Reason,
                                updateCheck.BranchCode,
                                updateCheck.ClearingOfficer
                            };

                            value.CheckImageStat = updateCheck.CheckStatus;

                            updateCheck.CheckStatus = value.VerifyAction;

                            _dBContext.Update(updateCheck);
                            _dBContext.SaveChanges();

                            SetInwardClearingChequeDetailsHistory(sParams);
                            _commonClass.Log(logMessage);
                            switch (_sendEmailTo)
                            {
                                case "CLR":
                                    _commonClass.SendEmailToClearing(sBranchCode,
                                        sReason: sReason,
                                        sSelectedCheck: sSelecteCheck);
                                    break;
                                case "BRN":
                                    _commonClass.SendEmailToBranch(sBranchCode,
                                        sReason: sReason,
                                        sSelectedCheck: sSelecteCheck);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    obj.StatusCode = "00";
                    obj.StatusMessage = "SUCCESS";
                }
                else
                {
                    obj.StatusMessage = "VerifyAction is required";
                }
                
            }
            catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }

        public async Task<ReturnGenericList<SelectListItem>> GetBranchList()
        {
            var obj = new ReturnGenericList<SelectListItem>();
            obj.StatusCode = "01";
            try
            {
                //var menuList = await _dBContext.BranchModels
                //                .Where(b => b.BranchBrstn.ToInt() > 0 && (branchCode.Contains(b.BranchCode)))
                //                .Select(e => new SelectListItem
                //                {
                //                    Value = e.UserTypeCode,
                //                    Text = e.UserTypeDesc,
                //                    IsChecked = false
                //                })
                //                .OrderBy(b => b.BranchBrstn)
                //                .ToListAsync();
                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
                //obj.Data = menuList;
            }
            catch(Exception ex) 
            { 
                obj.StatusMessage = ex.Message;
            }

            return obj;
        }

        public async Task<ReturnGenericList<SelectListItem>> GetReasonList()
        {
            var obj = new ReturnGenericList<SelectListItem>();
            obj.StatusCode = "01";
            try
            {
                var menuList = _commonClass.GetRejectReasonList().ToList(); // Ensure we execute the query and get a list
                List<SelectListItem> selectList = menuList.Select(reason => new SelectListItem
                {
                    Value = reason.RejectReasonCode, 
                    Text = reason.RejectReasonDesc
                }).ToList();

                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
                obj.Data = selectList;
            }
            catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }

            return obj;
        }

        public async Task<ReturnGenericList<SelectListItem>> GetOfficerList()
        {
            var obj = new ReturnGenericList<SelectListItem>();
            obj.StatusCode = "01";
            try
            {
                var userClaims = _userClaims.GetClaims();
                List<SelectListItem> list = new List<SelectListItem>();

                var userList1 = _commonClass.GetUserListByUserType("UsrClrSrVr", userClaims.UserID);
                var userList2 = _commonClass.GetUserListByUserType("UsrClrOff", userClaims.UserID);

                list.AddRange(userList1.Select(user => new SelectListItem
                {
                    Value = user.UserId.ToString(),
                    Text = user.UserDisplayName
                }));

                list.AddRange(userList2.Select(user => new SelectListItem
                {
                    Value = user.UserId.ToString(),
                    Text = user.UserDisplayName
                }));
                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
                obj.Data = (List<SelectListItem>)list;
            }
            catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }

            return obj;
        }

        public async Task<ReturnGenericData<CheckDetailModel>> GetICCDetails(int id)
        {
            var obj = new ReturnGenericData<CheckDetailModel>();
            obj.StatusCode = "01";
            try
            {
                //var detailCheck = await _dBContext.InwardClearingChequeDetailsModels.FindAsync(id);
                var detailCheck = await _dBContext.InwardClearingChequeDetailsModels.FirstOrDefaultAsync(e => e.Id == id);

                var getChequeImageFileContentType = GetChequeImageFileContentType(detailCheck.ChequeImageLinkedKey);
                var getChequeImageFileContent = GetChequeImageFileContent(detailCheck.ChequeImageLinkedKey);
                var getChequeImageFileContentR = await GetChequeImageFileContentR(detailCheck.ChequeImageLinkedKey);
                var getChequeImageFileContentF = await GetChequeImageFileContentF(detailCheck.ChequeImageLinkedKey);
                CheckDetailModel checkDetailModel = new CheckDetailModel()
                {
                    AccountNumber = detailCheck.AccountNumber,
                    CheckNumber = detailCheck.CheckNumber,
                    EffectivityDate = detailCheck.EffectivityDate,
                    CheckStatus = string.Format("{0} {1} {2}",
                                    _commonClass.GetStatusDesc(detailCheck.CheckStatus),
                                    //detailCheck.CheckStatus == "Open" ? "Open" : GetStatusList("", "").Find(l => l.Value == detailCheck.CheckStatus).Text,
                                    detailCheck.CheckStatus == "ReAssign" ? _commonClass.GetBranchOfAssignmentDesc(detailCheck.BranchCode) : "",
                                    detailCheck.CheckStatus == "ReferToOfficer" ? _commonClass.GetUserDisplayName(detailCheck.ClearingOfficer.GetValue()) : ""),

                    Reason = string.Format("{0}", detailCheck.ReasonDesc),//string.Format("{0}", detailCheck.CheckStatus == "Reject" || detailCheck.CheckStatus == "ReAssign" || detailCheck.CheckStatus == "ReferToOfficer" ? cmnCls.GetRejectReasonDesc(detailCheck.Reason): ""),
                    VerifiedBy = detailCheck.VerifiedBy,
                    VerifiedDateTime = detailCheck.VerifiedDateTime.GetDateValue(),
                    ApprovedBy = detailCheck.CheckStatus == "ReAssign" ? detailCheck.ClearingOfficer ?? detailCheck.ApprovedBy : detailCheck.ApprovedBy,
                    ApprovedDateTime = detailCheck.ApprovedDateTime.GetDateValue(),
                    ChequeImageFileContentType = getChequeImageFileContentType, //GetChequeImageFileContentType(detailCheck.ChequeImageLinkedKey),
                    ChequeImageFileContent = getChequeImageFileContent, //GetChequeImageFileContent(detailCheck.ChequeImageLinkedKey),
                    ChequeImageFileContentR = getChequeImageFileContentR, //GetChequeImageFileContentR(detailCheck.ChequeImageLinkedKey),
                    ChequeImageFileContentF = getChequeImageFileContentF, //GetChequeImageFileContentF(detailCheck.ChequeImageLinkedKey),
                    ChequeStats = _commonClass.GetDetailsFromHistory(detailCheck.ChequeImageLinkedKey)
                };

                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
                obj.Data = checkDetailModel;
            }
            catch(Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }

            return obj; 
        }


        public async Task<ReturnGenericData<ReturnCheckImageDetailTransaction>> GetCheckImageDetails(string sKey)
        {
            var obj = new ReturnGenericData<ReturnCheckImageDetailTransaction>();
            obj.StatusCode = "01";
            try
            {
                if (sKey.IsNull())
                {
                    obj.StatusMessage = "Invalid request";
                }
                else
                {
                    List<string> sFilenames = GetChequeImageFileNames(sKey);

                    string sAmount = "",
                            sFilename = sFilenames.Count > 0 ? sFilenames[0] : "",
                            sCheckStatus = GetInwardClearingChequeDetailsCheckStatus(sKey);

                    AccountDetails accountDetails = GetChequeAccountDetails(sKey);

                    var vFilename = sFilename.Split('_');

                    switch (vFilename.Length)
                    {
                        case 4:
                        case 5:
                            sFilename = string.Format("{0}_{1}_{2}", vFilename[0], vFilename[1], vFilename[2]);
                            sAmount = string.Format("{0:#,##0.00}", vFilename[3].ToDouble());
                            break;
                        default:
                            sFilename = sFilename.Length > 2 ? sFilename.Remove(sFilename.Length - 2, 2) : sFilename;
                            break;
                    }
                    ReturnCheckImageDetailTransaction data = 
                    new ReturnCheckImageDetailTransaction
                    {
                        FileContentType = GetChequeImageFileContentType(sKey),
                        FileContentTypeR = GetChequeImageFileContentTypeR(sKey),
                        FileContentTypeF = GetChequeImageFileContentTypeF(sKey),
                        FileContent = GetChequeImageFileContent(sKey),
                        FileContentR = await GetChequeImageFileContentR(sKey),
                        FileContentF = await GetChequeImageFileContentF(sKey),
                        FileName = string.Format("{0}", sFilename),
                        Amount = sAmount,
                        CheckStatus = sCheckStatus,
                        AccountName = accountDetails.AccountName,
                        AccountStatus = accountDetails.AccountStatus,
                        RejectReason = GetReasonFromHistory(sKey, sCheckStatus).GetAwaiter().GetResult()
                    };
                    obj.StatusCode = "00";
                    obj.StatusMessage = "SUCCESS";
                    obj.Data = data;
                }
            }
            catch(Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }

            return obj;
        }

        public async Task<ReturnGenericData<ReturnViewSignatures>> GetViewSignatures(string accountNo)
        {
            var obj = new ReturnGenericData<ReturnViewSignatures>();
            obj.StatusCode = "01";
            try
            {
                if (accountNo.IsNull() || accountNo == "")
                {
                    obj.StatusMessage = "Account is empty";
                }
                else
                {
                    string branchCode = _commonClass.GetBranchCodeViaBRSTN(accountNo).TrimStart('0');
                    if (branchCode == "")
                    {
                        obj.StatusMessage = "Branch Not Found";
                    }
                    else
                    {
                        var userClaims = _userClaims.GetClaims();
                        string userId = userClaims.UserID;
                        string userType = userClaims.UserType;
                        string userBranch = userClaims.BranchCode;
                        string userBranchBRSTN = _commonClass.GetBranchBRSTN(userBranch);

                        string userBranchBuddy = userClaims.BuddyCode;
                        string userBranchBuddyName = userClaims.BuddyBranch;
                        string userBranchBuddyBRSTN = _commonClass.GetBranchBRSTN(userBranchBuddy);

                        var defaultRule = new SvsRule
                        {
                            ACCNo = accountNo,
                            Amount = 0,
                            Reason = "Please Update Rule.",
                            RuleNo = Int16.Parse("0")
                        };

                        var defaultImageView = new ImageView
                        {
                            AccNo = accountNo,
                            Signature = string.Format("data: image/jpeg; base64 , {0}", GetNoSignature()),
                            EffectiveDate = DateTime.Parse("1900/01/01"),
                            ExpiredDate = DateTime.Parse("1900/01/01"),
                            Name = "Please update Signatures.",
                            Title = "Please update Signatures.",
                            IDCard = "Please update Signatures.",
                            Phone = "Please update Signatures.",
                            ImageNo = Int16.Parse("0")
                        };

                        var defaultCustInfo = new CustInfo()
                        {
                            ACCNo = accountNo,
                            NumImage = "0",
                            DateLstUpdated = DateTime.Parse("1900/01/01")
                        };
                        var defaultSVSGroup = new SvsGroup()
                        {
                            ACCNo = accountNo,
                            GroupName = "",
                            ImageNo = 0
                        };

                        string dBase = string.Format("boc_svs{0}", branchCode);


                        var custInfoList = new List<CustInfo>();
                        var svsGroupList = new List<SvsGroup>();
                        var svsRuleList = new List<SvsRule>();
                        var imageList = new List<ImageView>();

                        var conn = _config.GetSection("ConnectionStrings:PRDSVSContext").Get<string>().Decrypt();
                        _contextSVS.Database.GetDbConnection().ConnectionString = string.Format(conn, dBase);
                        bool canConnect = _contextSVS.Database.CanConnectAsync().GetAwaiter().GetResult();

                        var countCustInfoImg = await _contextFinacleSVS.SignOtherInfo
                                    .Where(a => a.acctid.ToString() == accountNo)
                                    .Join(_contextFinacleSVS.SignCustInfo,
                                          a => a.signid,
                                          b => b.signid,
                                          (a, b) => new { a, b })
                                    .CountAsync();
                        if(countCustInfoImg > 0)
                        {
                            custInfoList = await _contextFinacleSVS.SignOtherInfo
                                                            .Where(x => x.acctid.ToString() == accountNo)
                                                            .Join(_contextFinacleSVS.SignCustInfo, a => a.signid, b => b.signid,
                                                            (a, b) => new CustInfo
                                                            {
                                                                ACCNo = accountNo,
                                                                NumImage = countCustInfoImg.ToString(),                                                                
                                                                DateLstUpdated = b.createddate // b.modifieddate.IsNull() ? b.createddate : b.modifieddate
                                                                //error pag modifieddate is null
                                                            })
                                            .Take(1)
                                            .ToListAsync();
                            imageList = await _contextFinacleSVS.SignOtherInfo
                                        .Where(x => x.acctid.ToString() == accountNo)
                                        .Join(_contextFinacleSVS.SignCustInfo,
                                            a => a.signid,
                                            b => b.signid,
                                            (a, b) => new { a, b })
                                        .Join(_contextFinacleSVS.SignMaintenance,
                                            ab => ab.a.signid,
                                            c => c.signid,
                                            (ab, c) => new ImageView
                                            {
                                                AccNo = accountNo,
                                                Signature = "", //c.sign,  error pag balik
                                                EffectiveDate = c.effectivedate,
                                                ExpiredDate = c.expirydate,
                                                Name = ab.b.custname,
                                                Title = "Title",
                                                IDCard = "ab.b.idcard",
                                                Phone = "ab.b.phone",
                                                ImageNo = countCustInfoImg
                                            })
                                        .Take(1)
                                        .ToListAsync();
                        }
                        else
                        {
                            custInfoList.Add(defaultCustInfo);
                            imageList.Add(defaultImageView);
                            svsGroupList.Add(defaultSVSGroup);
                            svsRuleList.Add(defaultRule);
                        }

                        //if (canConnect)
                        //{
                        //    //custInfoList = _contextSVS.CustInfo.Where(e => e.ACCNo == accountNo).ToList();
                        //    svsGroupList = _contextSVS.SvsGroup.Where(e => e.ACCNo == accountNo).ToList();
                        //    svsRuleList = _contextSVS.SvsRule.Where(e => e.ACCNo == accountNo)
                        //                      .OrderBy(e => e.RuleNo)
                        //                      .ToList();
                        //
                        //    //imageList = _contextSVS.Image.Where(e => e.ACCNo == accountNo && e.ExpiredDate >= DateTime.Now.Date)
                        //    //        .Select( e => new ImageView {
                        //    //            AccNo = e.ACCNo,
                        //    //            Signature = string.Format("data: image/jpeg; base64 , {0}", e.Signature.ByteArrayToPngBase64String()),
                        //    //            EffectiveDate = e.EffectiveDate,
                        //    //            ExpiredDate = e.ExpiredDate,
                        //    //            Name = e.Name,
                        //    //            Title = e.Title,
                        //    //            IDCard = e.IDCard,
                        //    //            Phone = e.Phone,
                        //    //            ImageNo = e.ImageNo
                        //    //        })
                        //    //    .ToList();
                        //
                        //    if (imageList.Count == 0)
                        //        imageList.Add(defaultImageView);
                        //
                        //    if (svsRuleList.Count == 0)
                        //        svsRuleList.Add(defaultRule);
                        //}
                        //else
                        //{
                        //    svsGroupList.Add(defaultSVSGroup);
                        //    svsRuleList.Add(defaultRule);
                        //}

                        var data  = new ReturnViewSignatures()
                        {
                            Info = custInfoList,
                            Images = imageList,
                            Groups = svsGroupList,
                            Rules = svsRuleList
                        };

                        obj.StatusCode = "00";
                        obj.StatusMessage = "SUCCESS";
                        obj.Data = data;
                    }
                }
            }
            catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }

        public async Task<ReturnGenericList<SelectListItem>> GetUserAllowedAccess(string? key, string amount)
        {
            var obj = new ReturnGenericList<SelectListItem>();
            obj.StatusCode = "01";
            try
            {

                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userName = userClaims.DisplayName;
                string userType = userClaims.UserType;
                string userBranch = userClaims.BranchCode;
                string userBranchName = userClaims.Branch;
                string userBranchBRSTN = _commonClass.GetBranchBRSTN(userBranch);

                string userBranchBuddy = userClaims.BuddyCode;
                string userBranchBuddyName = userClaims.BuddyBranch;
                string userBranchBuddyBRSTN = _commonClass.GetBranchBRSTN(userBranchBuddy);
                bool branchBCP = _commonClass.GetBCPMode(userBranch);


                var lst = GetActionList(userType);
                if (userType.ToLower().Contains("branch"))
                {
                    if (key.IsNull() && branchBCP == false)
                        lst.Clear();
                }
                else
                {
                    if (userType.ToLower().Contains("clearing"))
                    {
                        //string sUserLimits = cmnCls.GetUserLimits(User.Claims.SingleOrDefault(e => e.Type == "UserID").GetValue());
                        string sUserLimits = _commonClass.GetUserLimits(userId);
                        string sMaxAmt = _commonClass.GetUserAllowedAmount(sUserLimits);
                        string sAllowed = _commonClass.GetUserAllowedAction(sUserLimits);
                        if (sUserLimits != "")
                        {
                            if (double.Parse(amount) >= double.Parse(sMaxAmt))
                            {
                                lst.Clear();
                                string[] tmpList = sAllowed.Split(',');
                                if (Array.IndexOf(tmpList, "AC") > -1)
                                    lst.Add(new SelectListItem { Text = "Accept", Value = "Accept" });
                                if (Array.IndexOf(tmpList, "RJ") > -1)
                                    lst.Add(new SelectListItem { Text = "Reject", Value = "Reject" });
                                if (Array.IndexOf(tmpList, "RB") > -1)
                                    lst.Add(new SelectListItem { Text = "Re-Assign to Branch", Value = "ReAssign" });
                                if (Array.IndexOf(tmpList, "RO") > -1)
                                    lst.Add(new SelectListItem { Text = "Next Level Approver", Value = "ReferToOfficer" });
                            }
                        }
                    }
                }
                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
                obj.Data = lst;
            }
            catch(Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }
            return obj;
        }
        
        private int GetInwardClearingChequeDetailsID(string chequeImageLinkedKey)
        {
            //InwardClearingChequeDetailsModel result = _dBContext.InwardClearingChequeDetailsModels.LastOrDefault(e => e.ChequeImageLinkedKey == chequeImageLinkedKey);
            InwardClearingChequeDetailsModel result = _dBContext.InwardClearingChequeDetailsModels
                                                    .Where(e => e.ChequeImageLinkedKey == chequeImageLinkedKey)
                                                    .OrderBy(e => e.Id)
                                                    .LastOrDefault();

            if (!result.IsNull())
                return result.Id;

            return 0;
        }

        private bool AllowedStatusUpdate(string fromStatus, string toStatus)
        {
            bool result = false;
            string[] acceptLst = new string[] { "Open", "ReAssign", "ReferToOfficer" };
            string[] rejectLst = new string[] { "Open", "ReAssign", "ReferToOfficer" };
            string[] reassignLst = new string[] { "Open", "ReferToOfficer" };
            string[] referLst = new string[] { "Open", "ReferToOfficer" };


            switch (toStatus)
            {
                case "Accept":
                    result = Array.IndexOf(acceptLst, fromStatus) > -1 ? true : false;
                    break;
                case "Reject":
                    result = Array.IndexOf(rejectLst, fromStatus) > -1 ? true : false; ;
                    break;
                case "ReAssign":
                    result = Array.IndexOf(reassignLst, fromStatus) > -1 ? true : false; ;
                    break;
                case "ReferToOfficer":
                    result = Array.IndexOf(referLst, fromStatus) > -1 ? true : false; ;
                    break;
            }

            return result;
        }

        private void SetInwardClearingChequeDetailsHistory(string[] sParams)
        {

            var userClaims = _userClaims.GetClaims();
            InwardClearingChequeHistoryModel inwardClearingChequeHistory = new InwardClearingChequeHistoryModel
            {
                ChequeImageLinkedKey = sParams[0],
                CheckStatusFrom = sParams[1],
                CheckStatusTo = sParams[2],
                Reason = sParams[3],
                BranchCode = sParams[4],
                ClearingOfficer = sParams[5],
                ActionBy = userClaims.UserID,// User.Claims.SingleOrDefault(e => e.Type == "UserID").GetValue(),
                ActionDateTime = DateTime.Now
            };

            _dBContext.Add(inwardClearingChequeHistory);
            _dBContext.SaveChanges();
        }

        private IQueryable<InwardClearingChequeDetailsModel> GetInwardClearingChequeDetailsModelFiltered(
                                        string key,
                                        string accountNoFilter,
                                        string sBRSTNFilter,
                                        string amountFromFilter,
                                        string amountToFilter,
                                        DateTime dateFrom,
                                        DateTime dateTo)
        {

            sBRSTNFilter = sBRSTNFilter.Trim();

            //SetUserClaims();

            var userClaims = _userClaims.GetClaims();
            string userId = userClaims.UserID;
            string userName = userClaims.DisplayName;
            string userType = userClaims.UserType;
            string userBranch = userClaims.BranchCode;
            string userBranchName = userClaims.Branch;
            string userBranchBRSTN = _commonClass.GetBranchBRSTN(userBranch);

            string userBranchBuddy = userClaims.BuddyCode;
            string userBranchBuddyName = userClaims.BuddyBranch;
            string userBranchBuddyBRSTN = _commonClass.GetBranchBRSTN(userBranchBuddy);

            //string userId = User.Claims.SingleOrDefault(e => e.Type == "UserID").GetValue();
            //string userBranch = User.Claims.SingleOrDefault(e => e.Type == "BranchCode").GetValue();
            //string userBranchBuddy = User.Claims.SingleOrDefault(e => e.Type == "BuddyCode").GetValue();

            IQueryable<InwardClearingChequeDetailsModel> res = _dBContext.InwardClearingChequeDetailsModels.OrderBy(e => e.CheckAmount);

            if (amountFromFilter != "" && amountToFilter != "")
            {
                if (amountFromFilter == amountToFilter)
                {
                    res = res.Where(e => e.CheckAmount >= amountFromFilter.ToDouble(0));
                }
                else
                {
                    res = res.Where(e => e.CheckAmount >= amountFromFilter.ToDouble(0) && e.CheckAmount <= amountToFilter.ToDouble(0));
                }
            }

            if (accountNoFilter != "")
                res = res.Where(e => e.AccountNumber.StartsWith(accountNoFilter));

            if (sBRSTNFilter != "")
                res = res.Where(e => e.Brstn.StartsWith(sBRSTNFilter));

            if (key == "OP")
                res = res.Where(e => e.CheckStatus.Equals("Open")
                                    && e.EffectivityDate.Date >= dateFrom && e.EffectivityDate.Date <= dateTo);
            else
                if (key == "RB")
                res = res.Where(e => e.CheckStatus.Equals("ReAssign")
                                && e.VerifiedDateTime.Date >= dateFrom && e.VerifiedDateTime.Date <= dateTo
                                && (e.BranchCode == userBranch || e.BranchCode == userBranchBuddy));
            else
                    if (key == "RO")
                res = res.Where(e => e.CheckStatus.Equals("ReferToOfficer")
                                    && e.VerifiedDateTime.Date >= dateFrom && e.VerifiedDateTime.Date <= dateTo
                                    && e.ClearingOfficer == userId);
            else
                        if (key == "AC")
                res = res.Where(e => e.CheckStatus.Equals("Accept")
                                    && e.ApprovedDateTime.Date >= dateFrom && e.ApprovedDateTime.Date <= dateTo);
            else
                            if (key == "RJ")
                res = res.Where(e => e.CheckStatus.Equals("Reject")
                                    && e.ApprovedDateTime.Date >= dateFrom && e.ApprovedDateTime.Date <= dateTo);
            else
                                if (key == "ALL")
                res = res.Where(e => e.EffectivityDate.Date >= dateFrom && e.EffectivityDate.Date <= dateTo);
            else
                res = res.Where(e => e.EffectivityDate.Date >= dateFrom && e.EffectivityDate.Date <= dateTo);

            return res;
        }

        private async void UpdateChequeImageLinkedKey(int id)
        {
            InwardClearingChequeDetailsModel updateCheck =
                await _dBContext.InwardClearingChequeDetailsModels.FindAsync(id);
            if (updateCheck != null)
            {
                if (updateCheck.ChequeImageLinkedKey != updateCheck.GenChequeImageLinkedKey)
                {
                    updateCheck.ChequeImageLinkedKey = updateCheck.GenChequeImageLinkedKey;
                    _dBContext.Update(updateCheck);
                    _dBContext.SaveChanges();
                }
            }
        }

        private string GetChequeImageFileContentType(string chequeImageLinkedKey)
        {
            try
            {
                InwardClearingChequeImageModel result = _dBContext.InwardClearingChequeImageModels.
                    LastOrDefault(e => e.ChequeImageLinkedKey == chequeImageLinkedKey);
                if (!result.IsNull())
                    return result.ChequeImageFileContentType;
            }
            catch { }
            return "";
        }

        private string GetChequeImageFileContent(string chequeImageLinkedKey)
        {
            //InwardClearingChequeImageModel result = _dBContext.InwardClearingChequeImageModels.
            //    LastOrDefault(e => e.ChequeImageLinkedKey == chequeImageLinkedKey);
            InwardClearingChequeImageModel result = _dBContext.InwardClearingChequeImageModels
                                                    .OrderByDescending(e => e.Id) 
                                                    .LastOrDefault(e => e.ChequeImageLinkedKey == chequeImageLinkedKey);
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
            //string[] fileSuffixFront = _config.GetSection("ChequeFileName:PrefixFront").Get<string[]>(); //_commonClass.GetConfig().ChequeFileName.PrefixFront.ToArray();
            //InwardClearingChequeImageModel result = null;
            //foreach (var suffix in fileSuffixFront)
            //{
            //    result = _dBContext.InwardClearingChequeImageModels
            //            .OrderByDescending(e => e.Id)
            //            .LastOrDefault(e => e.ChequeImageLinkedKey == string.Format("{0}_{1}", chequeImageLinkedKey, suffix));

            //    if (!result.IsNull())
            //        break;
            //}
            //if (!result.IsNull())
            //    return (result.ChequeImageFileContent.IsBase64String())
            //        ? (forReport) ? result.ChequeImageFileContent.FromBase64String().ByteArrayToPngBase64String() : string.Format("data:{0}; base64 , {1}", result.ChequeImageFileContentType, result.ChequeImageFileContent.FromBase64String().ByteArrayToPngBase64String())
            //        : (forReport) ? string.Format("{0}\\{1}", result.ChequeImageFileContent.Replace('|', '\\'), chequeImageLinkedKey) : string.Format("/CheckImages/{0}/{1}", result.ChequeImageFileContent.Replace('|', '/'), chequeImageLinkedKey);

            //string noImage = GetNoChequeImage();
            //return (forReport) ? noImage : string.Format("data: image/jpeg; base64 , {0}", noImage);
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
            //string[] fileSuffixBack = _config.GetSection("ChequeFileName:PrefixBack").Get<string[]>(); //_commonClass.GetConfig().ChequeFileName.PrefixBack;
            //InwardClearingChequeImageModel result = null;
            //foreach (var suffix in fileSuffixBack)
            //{
            //    result = _dBContext.InwardClearingChequeImageModels
            //            .OrderByDescending(e => e.Id)
            //            .LastOrDefault(e => e.ChequeImageLinkedKey == string.Format("{0}_{1}", chequeImageLinkedKey, suffix));

            //    if (!result.IsNull())
            //        break;
            //}
            //if (!result.IsNull())
            //    return (result.ChequeImageFileContent.IsBase64String())
            //        ? (forReport) ? result.ChequeImageFileContent.FromBase64String().ByteArrayToPngBase64String() : string.Format("data:{0}; base64 , {1}", result.ChequeImageFileContentType, result.ChequeImageFileContent.FromBase64String().ByteArrayToPngBase64String())
            //        : (forReport) ? string.Format("{0}\\{1}", result.ChequeImageFileContent.Replace('|', '\\'), chequeImageLinkedKey) : string.Format("/CheckImages/{0}/{1}", result.ChequeImageFileContent.Replace('|', '/'), chequeImageLinkedKey);

            //string noImage = GetNoChequeImage();
            //return (forReport) ? noImage : string.Format("data: image/jpeg; base64 , {0}", noImage);
        }

        private string GetNoChequeImage() { return "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"; }//"noimages.png".ReadResource(true); }
        private string GetNoSignature() { return "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owACAAAAAABsRwEEAAAAAADYjgIIAAAAAACwHQUQAAAAAABgOwogAAAAAADAdhRAAAAAAACA7SiAAAAAAAAA21EAAQAAAAAAtqMAAgAAAAAAbEcBBAAAAAAA2I4CCAAAAAAAsB0FEAAAAAAAYDsKIAAAAAAAwHYUQAAAAAAAgO0ogAAAAAAAANtRAAEAAAAAALajAAIAAAAAAGxHAQQAAAAAANiOAggAAAAAALAdBRAAAAAAAGA7CiAAAAAAAMB2FEAAAAAAAIDtKIAAAAAAAADbUQABAAAAAAC2owACAAAAAABsRwEEAAAAAADYjgIIAAAAAACwHQUQAAAAAABgOwogAAAAAADAdhRAAAAAAACA7SiAAAAAAAAA21EAAQAAAAAAtqMAAgAAAAAAbEcBBAAAAAAA2I4CCAAAAAAAsB0FEAAAAAAAYDsKIAAAAAAAwHYUQAAAAAAAgO0ogAAAAAAAANtRAAEAAAAAALajAAIAAAAAAGxHAQQAAAAAANiOAggAAAAAALAdBRAAAAAAAGA7CiAAAAAAAMB2FEAAAAAAAIDtKIAAAAAAAADbUQABAAAAAAC2owACAAAAAABsRwEEAAAAAADYjgIIAAAAAACwHQUQAAAAAABgOwogAAAAAADAdhRAAAAAAACA7SiAAAAAAAAA21EAAQAAAAAAtqMAAgAAAAAAbEcBBAAAAAAA2I4CCAAAAAAAsB0FEAAAAAAAYDsKIAAAAAAAwHYUQAAAAAAAgO0ogAAAAAAAANtRAAEAAAAAALajAAIAAAAAAGxHAQQAAAAAANiOAggAAAAAALAdBRAAAAAAAGA7CiAAAAAAAMB2FEAAAAAAAIDtKIAAAAAAAADbUQABAAAAAAC2owACAAAAAABsRwEEAAAAAADYjgIIAAAAAACwHQUQAAAAAABgOwogAAAAAADAdhRAAAAAAACA7SiAAAAAAAAA21EAAQAAAAAAtqMAAgAAAAAAbEcBBAAAAAAA2I4CCAAAAAAAsB0FEAAAAAAAYDsKIAAAAAAAwHYUQAAAAAAAgO0ogAAAAAAAANtRAAEAAAAAALajAAIAAAAAAGxHAQQAAAAAANiOAggAAAAAALAdBRAAAAAAAGA7CiAAAAAAAMB2FEAAAAAAAIDtKIAAAAAAAADbUQABAAAAAAC2owACAAAAAABsRwEEAAAAAADYjgIIAAAAAACwHQUQAAAAAABgOwogAAAAAADAdhRAAAAAAACA7SiAAAAAAAAA21EAAQAAAAAAtqMAAgAAAAAAbEcBBAAAAAAA2I4CCAAAAAAAsB0FEAAAAAAAYDsKIAAAAAAAwHYUQAAAAAAAgO0ogAAAAAAAANtRAAEAAAAAALajAAIAAAAAAGxHAQQAAAAAANiOAggAAAAAALAdBRAAAAAAAGA7CiAAAAAAAMB2FEAAAAAAAIDtKIAAAAAAAADbUQABAAAAAAC2owACAAAAAABsRwEEAAAAAADYjgIIAAAAAACwHQUQAAAAAABgOwogAAAAAADAdhRAAAAAAACA7SiAAAAAAAAA21EAAQAAAAAAtqMAAgAAAAAAbEcBBAAAAAAA2I4CCAAAAAAAsB0FEAAAAAAAYDsKIAAAAAAAwHYUQAAAAAAAgO0ogAAAAAAAANtRAAEAAAAAALajAAIAAAAAAGxHAQQAAAAAANiOAggAAAAAALAdBRAAAAAAAGA7CiAAAAAAAMB2FEAAAAAAAIDtKIAAAAAAAADbUQABAAAAAAC2owACAAAAAABsRwEEAAAAAADYjgIIAAAAAACwHQUQAAAAAABgOwogAAAAAADAdhRAAAAAAACA7SiAAAAAAAAA21EAAQAAAAAAtqMAAgAAAAAAbEcBBAAAAAAA2I4CCAAAAAAAsB0FEAAAAAAAYDsKIAAAAAAAwHYUQAAAAAAAgO0ogAAAAAAAANtRAAEAAAAAALajAAIAAAAAAGxHAQQAAAAAANiOAggAAAAAALAdBRAAAAAAAGA7CiAAAAAAAMB2FEAAAAAAAIDtKIAAAAAAAADbUQABAAAAAAC2owACAAAAAABsRwEEAAAAAADYjgIIAAAAAACwHQUQAAAAAABgOwogAAAAAADAdhRAAAAAAACA7SiAAAAAAAAA21EAAQAAAAAAtqMAAgAAAAAAbEcBBAAAAAAA2I4CCAAAAAAAsB0FEAAAAAAAYDsKIAAAAAAAwHYUQAAAAAAAgO0ogAAAAAAAANtRAAEAAAAAALajAAIAAAAAAGxHAQQAAAAAANiOAggAAAAAALAdBRAAAAAAAGA7CiAAAAAAAMB2FEAAAAAAAIDtKIAAAAAAAADbUQABAAAAAAC2owACAAAAAABsRwEEAAAAAADYjgIIAAAAAACwHQUQAAAAAABgOwogAAAAAADAdhRA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"; }//"nosignatures.png".ReadResource(true); }

        private List<string> GetChequeImageFileNames(string chequeImageLinkedKey)
        {
            List<string> lResult = new List<string> { };
            List<InwardClearingChequeImageModel> result = _dBContext.InwardClearingChequeImageModels
                                            .Where(e => e.ChequeImageLinkedKey
                                            .StartsWith(chequeImageLinkedKey)).ToList();
            if (!result.IsNull())
            {
                foreach (InwardClearingChequeImageModel item in result)
                {
                    lResult.Add(item.ChequeImageFileName);
                }
            }

            return lResult;
        }

        private string GetInwardClearingChequeDetailsCheckStatus(string chequeImageLinkedKey)
        {
            InwardClearingChequeDetailsModel result = _dBContext.InwardClearingChequeDetailsModels
                                                    .OrderByDescending(e => e.Id)
                                                    .LastOrDefault(e => e.ChequeImageLinkedKey == chequeImageLinkedKey);
            if (!result.IsNull())
                return result.CheckStatus;

            return "";
        }

        private AccountDetails GetChequeAccountDetails(string chequeImageLinkedKey)
        {
            ChequeAccountDetail result = _dBContext.ChequeAccountDetails.OrderByDescending(e => e.Id).LastOrDefault(e => e.ChequeImageLinkedKey == chequeImageLinkedKey);

            if (!result.IsNull())
                return new AccountDetails() { AccountName = result.AccountName, AccountStatus = result.AccountStatus };

            return new AccountDetails() { AccountName = "", AccountStatus = "" };
        }

        private string GetChequeImageFileContentTypeR(string chequeImageLinkedKey)
        {
            try
            {
                //chequeImageLinkedKey = string.Format("{0}{1}", chequeImageLinkedKey, "_R");
                //InwardClearingChequeImageModel result = _context.InwardClearingChequeImageModel.
                //    LastOrDefault(e => e.ChequeImageLinkedKey == chequeImageLinkedKey);
                string[] fileSuffixBack = _config.GetSection("ChequeFileName:PrefixBack").Get<string[]>();
                InwardClearingChequeImageModel result = null;
                foreach (var suffix in fileSuffixBack)
                {
                    result = _dBContext.InwardClearingChequeImageModels
                            .OrderByDescending(e => e.Id)
                            .LastOrDefault(e => e.ChequeImageLinkedKey == string.Format("{0}_{1}", chequeImageLinkedKey, suffix));

                    if (!result.IsNull())
                        break;
                }
                if (!result.IsNull())
                    return result.ChequeImageFileContentType;
            }
            catch { }
            return "";
        }

        private string GetChequeImageFileContentTypeF(string chequeImageLinkedKey)
        {
            try
            {
                //chequeImageLinkedKey = string.Format("{0}{1}", chequeImageLinkedKey, "_F");
                //InwardClearingChequeImageModel result = _context.InwardClearingChequeImageModel.
                //    LastOrDefault(e => e.ChequeImageLinkedKey == chequeImageLinkedKey);
                string[] fileSuffixFront = _config.GetSection("ChequeFileName:PrefixFront").Get<string[]>();
                InwardClearingChequeImageModel result = null;
                foreach (var suffix in fileSuffixFront)
                {
                    result = _dBContext.InwardClearingChequeImageModels
                            .OrderByDescending(e => e.Id)
                            .LastOrDefault(e => e.ChequeImageLinkedKey == string.Format("{0}_{1}", chequeImageLinkedKey, suffix));

                    if (!result.IsNull())
                        break;
                }
                if (!result.IsNull())
                    return result.ChequeImageFileContentType;
            }
            catch { }
            return "";
        }

        private async Task<string> GetReasonFromHistory(string chequeImageLinkedKey, string checkStatus)
        {
            InwardClearingChequeHistoryModel result =
                await _dBContext.InwardClearingChequeHistoryModels
                    .OrderByDescending(e => e.Id)
                    .LastOrDefaultAsync(e => e.ChequeImageLinkedKey == chequeImageLinkedKey && e.CheckStatusTo == checkStatus);

            if (!result.IsNull())
                return _commonClass.GetRejectReasonDesc(result.Reason);

            return "";
        }

        private List<SelectListItem> GetActionList(string userType)
        {
            var lst = new List<SelectListItem>();
            if (userType.ToLower().Contains("clearing"))
            {
                lst = new List<SelectListItem> {
                    new SelectListItem { Text = "Accept", Value = "Accept" },
                    new SelectListItem { Text = "Reject", Value = "Reject" },
                    new SelectListItem { Text = "Re-Assign to Branch", Value = "ReAssign" },
                    new SelectListItem { Text = "Next Level Approver", Value = "ReferToOfficer" }
                };
            }
            if (userType.ToLower().Contains("branch"))
            {
                lst = new List<SelectListItem> {
                    new SelectListItem { Text = "Honor", Value = "Accept" },
                    new SelectListItem { Text = "Return", Value = "Reject" }
                };
            }
            return lst;
        }

        public async Task<string> GetChequeFile(string chequeImageLinkedKey, string[] fileSuffixFront, bool forReport)
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

    }
}
