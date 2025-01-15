using Application.Models;
//using Crypto.CryptoManager;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
using System.Configuration;

namespace Infrastructure.Common
{
    public class CommonClass
    {
        private readonly AppDbContext _dBContext;
        private readonly IConfiguration _config;
        public CommonClass(AppDbContext dBContext, IConfiguration config)
        {
            _dBContext = dBContext;
            _config = config;
        }

        public enum EmailFields
        {
            Error,
            SelectedCheck,
            Branch,
            Reason
        }

        #region GetData
        public string GetActionDesc(string str)
        {
            if (str == "")
                return "";
            else
            {
                Dictionary<string, string> data = MakeActionDictByKey();
                List<string> list = new List<string>();
                str.Split(',').ToList().ForEach(e => { list.Add(data[e]); });
                return string.Join(", ", list);
            }
        }
        private Dictionary<string, string> MakeActionDictByKey()
        {
            Dictionary<string, string> actionDict = new Dictionary<string, string>();
            actionDict.Add("AC", "Accept");
            actionDict.Add("RJ", "Reject");
            actionDict.Add("RB", "Refer to Branch");
            actionDict.Add("RO", "Next Level Approver");
            return actionDict;
        }
        public string GetUserDisplayName(string sUserID)
        {
            var userModel = _dBContext.UserModels.SingleOrDefault(e => e.UserId == sUserID);// && e.ISDeleted == false);
            if (userModel != null)
                return userModel.UserDisplayName.ToString();

            return "";
        }
        public Dictionary<string, string> GetUserLimitsDetails(string sCode)
        {
            var res = _dBContext.AmountLimitsModels.SingleOrDefault(e => e.AmountLimitsCode == sCode && e.Isdeleted == false);
            if (res != null)
            {
                return new Dictionary<string, string>
                {
                    {"LimitsDesc",res.AmountLimitsDesc },
                    {"LimitAmount",res.MaxAmountLimit.ToString() },
                    {"LimitAction",GetActionDesc(res.AllowedAction) }
                };
            }

            return new Dictionary<string, string>();
        }
        public string GetMenuIDsDesc(string sMenuIDs)
        {
            string sMenuIDsDesc = "";
            if (sMenuIDs.Contains(','))
            {
                foreach (string item in sMenuIDs.Split(",")) //root
                {

                    sMenuIDsDesc += GetMenuDesc(item) + "|";
                }
                sMenuIDsDesc = sMenuIDsDesc.Remove(sMenuIDsDesc.LastIndexOf('|'));
            }
            else
                sMenuIDsDesc = GetMenuDesc(sMenuIDs);

            //(sMenuIDsDesc.Last() == '|') ? sMenuIDsDesc.Substring(0, sMenuIDsDesc.Length - 1) : sMenuIDsDesc;

            return sMenuIDsDesc;
        }
        public string GetMenuDesc(string sMenuCode)
        {
            var menuModel = _dBContext.MenuModels.SingleOrDefault(e => e.MenuCode == sMenuCode && e.Isdeleted == false);
            if (menuModel != null)
                return menuModel.MenuDesc;

            return "No Menu Description Set";
        }
        public string GetGroupIDbyUserType(string sUserTypeCode)
        {
            var groupMemberModel = _dBContext.GroupMemberModels.SingleOrDefault(e => e.UserTypes.Contains(sUserTypeCode) && e.Isdeleted == false);
            if (groupMemberModel != null)
                return groupMemberModel.GroupId;

            return "";
        }
        public string GetGroupDesc(string sGroupCode)
        {
            var groupModel = _dBContext.GroupModels.SingleOrDefault(e => e.GroupCode == sGroupCode && e.Isdeleted == false);
            if (groupModel != null)
                return groupModel.GroupDesc;

            return sGroupCode;
        }
        public string GetUserTypeDesc(string sUserTypeCode)
        {
            var userTypeModel = _dBContext.UserTypeModels.SingleOrDefault(e => e.UserTypeCode == sUserTypeCode && e.Isdeleted == false);
            if (userTypeModel != null)
                return userTypeModel.UserTypeDesc;

            return sUserTypeCode;
        }
        public string GetBranchBRSTN(string sBranchCode)
        {
            var branchModel = _dBContext.BranchModels.SingleOrDefault(e => e.BranchCode == sBranchCode && e.Isdeleted == false);
            if (!branchModel.IsNull())
                return branchModel.BranchBrstn.GetValue();

            return "";
        }
        public string GetBranchOfAssignmentDesc(string sBranchCode)
        {
            var branchModel = _dBContext.BranchModels.SingleOrDefault(e => e.BranchCode == sBranchCode && e.Isdeleted == false);
            if (branchModel != null)
                return branchModel.BranchDesc;

            return "No Branch Of Assignment Description Set";
        }
        public string[] GetBuddyBranches(string sBranchCode)
        {
            string[] res = new string[] { };
            var branchModel = _dBContext.BranchModelAuxes.Where(e => e.BranchCode == sBranchCode);
            if (!branchModel.IsNull())
                res = branchModel.Select(e => e.BranchBuddyCode).ToArray();

            return res;
        }
        public List<SelectListItem> GetActionList()
        {
            List<SelectListItem> lstSelItem = new List<SelectListItem>();
            foreach (KeyValuePair<string, string> keyValue in MakeActionDictByKey())
            {
                lstSelItem.Add(new SelectListItem { Value = keyValue.Key, Text = keyValue.Value });
            };
            return lstSelItem;
        }
        //public bool ValidateADUser(string user)
        //{
        //    bool _bIsCredValid = false;

        //    using (PrincipalContext _oPrincipalContext = new PrincipalContext(ContextType.Domain, GetDomainName()))
        //    {
        //        var x = UserPrincipal.FindByIdentity(_oPrincipalContext, IdentityType.SamAccountName, user);
        //        _bIsCredValid = (!x == null);
        //    }
        //    return _bIsCredValid;
        //}


        //public bool IsNull(this object thisObj)
        //{
        //    return ReferenceEquals(null, thisObj);
        //}
        public string GetUserTypeIDsDesc(string sUserTypes)
        {
            string UserTypesDesc = "";
            if (sUserTypes.Contains(','))
            {
                foreach (string item in sUserTypes.Split(","))
                {
                    UserTypesDesc += GetUserTypeDesc(item) + "|";
                }
                UserTypesDesc = UserTypesDesc.Remove(UserTypesDesc.LastIndexOf('|'));
            }
            else
                UserTypesDesc = GetUserTypeDesc(sUserTypes);

            //UserTypesDesc = (UserTypesDesc.Last() == '|') ? UserTypesDesc.Substring(0, UserTypesDesc.Length - 1) : UserTypesDesc;

            return UserTypesDesc;
        }
        public string GetStatusDesc(string str)
        {
            Dictionary<string, KeyValuePair<string, string>> pairs = MakeActionDictByCode("All");
            return (str.IsNull()) ? "" : (pairs.ContainsKey(str)) ? pairs[str].Value : "";

        }
        public string GetRejectReasonDesc(string sRejectReasonCode)
        {
            var rejectReasonModel = _dBContext.RejectReasonModels.SingleOrDefault(e => e.RejectReasonCode == sRejectReasonCode && e.Isdeleted == false);
            if (!rejectReasonModel.IsNull())
                return rejectReasonModel.RejectReasonDesc;

            return "";
        }
        private Dictionary<string, KeyValuePair<string, string>> MakeActionDictByCode(string key)
        {
            Dictionary<string, KeyValuePair<string, string>> actionDict = new Dictionary<string, KeyValuePair<string, string>>();

            if (key == "All")
            {
                actionDict.Add("Open", new KeyValuePair<string, string>("Open", "Open"));
                actionDict.Add("Upload", new KeyValuePair<string, string>("Upload", "Upload"));
                actionDict.Add("Accept", new KeyValuePair<string, string>("AC", "Accept"));
                actionDict.Add("Reject", new KeyValuePair<string, string>("RJ", "Reject"));
                actionDict.Add("ReAssign", new KeyValuePair<string, string>("RB", "Re-Assign to Branch"));
                actionDict.Add("ReferToOfficer", new KeyValuePair<string, string>("RO", "Next Level Approver"));
            }

            if (key == "Branch")
            {
                actionDict.Add("Honor", new KeyValuePair<string, string>("AC", "Accept"));
                actionDict.Add("Return", new KeyValuePair<string, string>("RJ", "Reject"));
            }

            return actionDict;
        }
        public string GetBranchOfAssignment(string sBRSTN)
        {
            var branchModel = _dBContext.BranchModels.SingleOrDefault(e => e.BranchBrstn == sBRSTN && e.Isdeleted == false);
            if (!branchModel.IsNull())
                return branchModel.BranchCode.GetValue();

            return "";
        }
        public void Log(string sLog)
        {
            using (AppDbContext x = SetContext())
            {
                x.Add(new AuditLog() { Log = sLog, LogTime = DateTime.Now });
                x.SaveChanges();
                x.Dispose();
            }
        }
        public string GetMenus(string sMenuIDs)
        {
            string sMenuIDsDesc = "", sSubMenuDesc = "",
                   subItems = "";

            if (sMenuIDs.Contains(','))
            {
                foreach (string item in sMenuIDs.Split(",", StringSplitOptions.RemoveEmptyEntries)) //root
                {
                    sMenuIDsDesc += GetMenuDesc(item) + "@";
                    subItems = GetSubMenus(item);
                    if (subItems.Contains(','))
                    {
                        foreach (string subItem in subItems.Split(",", StringSplitOptions.RemoveEmptyEntries)) //sub
                        {
                            sSubMenuDesc += GetMenuDesc(subItem) + "#" + GetSubMenuController(subItem) + "$" + GetMenuActionMethod(subItem) + ",";
                            //if (subItem != "")
                            //{
                            //    sSubMenuDesc += GetMenuDesc(subItem) + "#" + GetSubMenuController(subItem) + "$" + GetMenuActionMethod(subItem) + ",";
                            //}
                        }
                        //if (sSubMenuDesc != "")
                        //    sSubMenuDesc = (sSubMenuDesc.Last() == ',') ? sSubMenuDesc.Substring(0, sSubMenuDesc.Length - 1) : sSubMenuDesc;

                        sSubMenuDesc = sSubMenuDesc.Remove(sSubMenuDesc.LastIndexOf(','));
                    }
                    else
                        sSubMenuDesc = GetMenuDesc(subItems) + "#" + GetSubMenuController(subItems) + "$" + GetMenuActionMethod(subItems);


                    sMenuIDsDesc += sSubMenuDesc + "|";
                    sSubMenuDesc = "";
                }
                // (sMenuIDsDesc.Last() == '|') ?  sMenuIDsDesc.Substring(0, sMenuIDsDesc.Length - 1) : sMenuIDsDesc;
                sMenuIDsDesc = sMenuIDsDesc.Remove(sMenuIDsDesc.LastIndexOf('|'));
            }
            else
            {
                sMenuIDsDesc = GetMenuDesc(sMenuIDs) + "@";  //root
                subItems = GetSubMenus(sMenuIDs);
                if (subItems.Contains(','))
                {
                    foreach (string subItem in subItems.Split(",", StringSplitOptions.RemoveEmptyEntries)) //sub
                    {
                        sSubMenuDesc += GetMenuDesc(subItem) + "#" + GetSubMenuController(subItem) + "$" + GetMenuActionMethod(subItem) + ",";
                    }

                    sSubMenuDesc = sSubMenuDesc.Remove(sSubMenuDesc.LastIndexOf(','));
                }
                else
                    sSubMenuDesc = GetMenuDesc(subItems) + "#" + GetSubMenuController(subItems) + "$" + GetMenuActionMethod(subItems);


                sMenuIDsDesc += sSubMenuDesc + "|";
                sSubMenuDesc = "";
            }

            return sMenuIDsDesc;
        }
        public string GetSubMenus(string sMenuCode)
        {
            var menuModel = _dBContext.MenuModels.SingleOrDefault(e => e.MenuCode == sMenuCode && e.Isdeleted == false);
            if (!menuModel.IsNull())
                return menuModel.SubMenus.GetValue();

            return "No Sub Menu Set";
        }
        public string GetSubMenuController(string sMenuCode)
        {
            var menuModel = _dBContext.MenuModels.SingleOrDefault(e => e.MenuCode == sMenuCode && e.Isdeleted == false);
            if (!menuModel.IsNull())
                return menuModel.Controller.GetValue();

            return "No Sub Menu Controller Set";
        }
        public string GetMenuActionMethod(string sMenuCode)
        {
            var menuModel = _dBContext.MenuModels.SingleOrDefault(e => e.MenuCode == sMenuCode && e.Isdeleted == false);
            if (!menuModel.IsNull())
                return string.Format("{0}{1}{2}", menuModel.ActionMethod.GetValue(), menuModel.ActionMethodParam.IsNull() ? "" : "?", menuModel.ActionMethodParam.GetValue());

            return "No Sub Menu Set";
        }
        public string GetGroupMenu(string sGroupID)
        {
            var groupAccessModel = _dBContext.GroupAccessModels.SingleOrDefault(e => e.GroupId == sGroupID && e.Isdeleted == false);
            if (!groupAccessModel.IsNull())
                return groupAccessModel.MenuIds.GetValue();

            return "";
        }
        public string GetBuddyBranch(string sBranchCode)
        {
            var branchModel = _dBContext.BranchModelAuxes.SingleOrDefault(e => e.BranchCode == sBranchCode);
            if (!branchModel.IsNull())
                return branchModel.BranchBuddyCode;

            return "";
        }
        public Dictionary<ChequeStats, string> GetDetailsFromHistory(string chequeImageLinkedKey)
        {
            Dictionary<ChequeStats, string> result = new Dictionary<ChequeStats, string>();
            result.Add(ChequeStats.Upload, "");
            result.Add(ChequeStats.Accept, "");
            result.Add(ChequeStats.Reject, "");
            result.Add(ChequeStats.ReAssign, "");
            result.Add(ChequeStats.ReferToOfficer, "");
            result.Add(ChequeStats.BrAccept, "");
            result.Add(ChequeStats.BrReject, "");
            result.Add(ChequeStats.COAccept, "");
            result.Add(ChequeStats.COReject, "");
            result.Add(ChequeStats.COReAssign, "");


            string[] fromStat = new string[] { "Upload", "Open", "Open", "Open", "Open", "ReAssign", "ReAssign", "ReferToOfficer", "ReferToOfficer", "ReferToOfficer" };
            string[] toStat = new string[] { "Open", "Accept", "Reject", "ReAssign", "ReferToOfficer", "Accept", "Reject", "Accept", "Reject", "ReAssign" };


            for (int ctr = 0; ctr < fromStat.Length; ctr++)
            {
                InwardClearingChequeHistoryModel res = _dBContext.InwardClearingChequeHistoryModels.Where(e => e.ChequeImageLinkedKey == chequeImageLinkedKey
                                                        && e.CheckStatusFrom == fromStat[ctr]
                                                        && e.CheckStatusTo == toStat[ctr]).OrderBy(x => x.Id).LastOrDefaultAsync().GetAwaiter().GetResult();
                if (!res.IsNull())
                {
                    switch (ctr)
                    {
                        case 0:
                            result[ChequeStats.Upload] = string.Format("Uploaded By {0} On {1}.", res.ActionBy, res.ActionDateTime);
                            break;
                        case 1:
                            result[ChequeStats.Accept] = string.Format("Accepted By {0} On {1}.", res.ActionBy, res.ActionDateTime);
                            break;
                        case 2:
                            result[ChequeStats.Reject] = string.Format("Rejected By {0} Reason {2}.", res.ActionBy, res.ActionDateTime, GetRejectReasonDesc(res.Reason));
                            break;
                        case 3:
                            result[ChequeStats.ReAssign] = string.Format("ReAssigned To Branch {3} By {0} Reason {2}.", res.ActionBy, res.ActionDateTime, GetRejectReasonDesc(res.Reason), GetBranchOfAssignmentDesc(res.BranchCode));
                            break;
                        case 4:
                            result[ChequeStats.ReferToOfficer] = string.Format("Next Level Approver {2} By {0} On {1}.", res.ActionBy, res.ActionDateTime, res.ClearingOfficer);
                            break;
                        case 5:
                            result[ChequeStats.BrAccept] = string.Format("Branch Accept By {0} On {1}.", res.ActionBy, res.ActionDateTime);
                            break;
                        case 6:
                            result[ChequeStats.BrReject] = string.Format("Branch Reject By {0} Reason {2}.", res.ActionBy, res.ActionDateTime, GetRejectReasonDesc(res.Reason));
                            break;
                        case 7:
                            result[ChequeStats.COAccept] = string.Format("Accepted By {0} On {1}.", res.ActionBy, res.ActionDateTime);
                            break;
                        case 8:
                            result[ChequeStats.COReject] = string.Format("Rejected By {0} Reason {2}.", res.ActionBy, res.ActionDateTime, GetRejectReasonDesc(res.Reason));
                            break;
                        case 9:
                            result[ChequeStats.COReAssign] = string.Format("ReAssigned To Branch {3} By {0} Reason {2}.", res.ActionBy, res.ActionDateTime, GetRejectReasonDesc(res.Reason), GetBranchOfAssignmentDesc(res.BranchCode));
                            break;
                    }

                }

            }

            return result;
        }
        public UserPrincipal GetADCredInfo(string user)
        {
            UserPrincipal _oUserPrincipal;
            using (PrincipalContext _oPrincipalContext = new PrincipalContext(ContextType.Domain, GetDomainName()))
            {
                _oUserPrincipal = UserPrincipal.FindByIdentity(_oPrincipalContext, IdentityType.SamAccountName, user);
            }
            return _oUserPrincipal;
        }

        public void SendEmailToClearing(string sBranchCode, string sReason = "", string sMessage = "", string sSelectedCheck = "")
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(_config["SMTPHost"], int.Parse(_config["SMTPPort"]));
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["EmailSender"])
                };
                mailMessage.To.Add(GetBranchEmail("ClrDept"));

                string sBranchEmail = GetBranchEmail(sBranchCode);
                if (sBranchEmail != "")
                    mailMessage.CC.Add(sBranchEmail);

                string emailBody = GetEmailBody("Clearing");

                emailBody = emailBody.Replace("[" + nameof(EmailFields.Error) + "]",
                                                sMessage ?? "");
                emailBody = emailBody.Replace("[" + nameof(EmailFields.SelectedCheck) + "]",
                                                sSelectedCheck ?? "");
                emailBody = emailBody.Replace("[" + nameof(EmailFields.Reason) + "]",
                                                sReason ?? "");

                mailMessage.Body = emailBody;
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = GetEmailSubject("Clearing");
                mailMessage.Priority = MailPriority.High;
                object usertoken = new object();
                smtpClient.SendAsync(mailMessage, usertoken);
            }
            catch (Exception ex)
            {
                Log(string.Format("Error Sending email [{2}] to Clearing: {0} {1}",
                    ex.Message,
                    ex.InnerException.IsNull() ? "" : ex.InnerException.ToString(),
                    sBranchCode
                    ));
            }
        }

        public void SendEmailToBranch(string sBranchCode, string sReason = "", string sMessage = "", string sSelectedCheck = "")
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(_config["SMTPHost"], int.Parse(_config["SMTPPort"]));
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["EmailSender"])
                };

                mailMessage.To.Add(GetBranchEmail(sBranchCode));

                string emailBody = GetEmailBody("Branch");

                emailBody = emailBody.Replace("[" + nameof(EmailFields.Branch) + "]",
                                                GetBranchOfAssignmentDesc(sBranchCode) ?? "");

                emailBody = emailBody.Replace("[" + nameof(EmailFields.Error) + "]",
                                                sMessage ?? "");
                emailBody = emailBody.Replace("[" + nameof(EmailFields.SelectedCheck) + "]",
                                                sSelectedCheck ?? "");
                emailBody = emailBody.Replace("[" + nameof(EmailFields.Reason) + "]",
                                                sReason ?? "");

                mailMessage.Body = emailBody;
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = GetEmailSubject("Branch");
                mailMessage.Priority = MailPriority.High;
                object usertoken = new object();
                smtpClient.SendAsync(mailMessage, usertoken);

            }
            catch (Exception ex)
            {
                Log(string.Format("Error Sending email to Branch[{2}]: {0} {1}",
                    ex.Message,
                    ex.InnerException.IsNull() ? "" : ex.InnerException.ToString(),
                    sBranchCode
                    ));
            }
        }

        public string GetNextChequeImageLinkedKey(string chequeImageLinkedKey,
                            string checkStatus, string filter, string dateFrom, string dateTo,
                            string branchCode = "", string clearingOfficer = "")
        {
            //accountNumberFilter + '|' + sBSRTNFilter + '|' + amountFromFilter + '|' + amountToFilter
            var filters = filter.Split('|');
            string accountNumberFilter = filter == "" ? "" : filters[0],
                sBSRTNFilter = filter == "" ? "" : filters[1],
                amountFromFilter = filter == "" ? "" : filters[2],
                amountToFilter = filter == "" ? "" : filters[3];

            IQueryable<InwardClearingChequeDetailsModel> res = _dBContext.InwardClearingChequeDetailsModels;

            if (accountNumberFilter.GetValue() != "")
                res = res.Where(e => e.AccountNumber.StartsWith(accountNumberFilter));

            if (sBSRTNFilter.GetValue() != "")
                res = res.Where(e => e.Brstn.StartsWith(sBSRTNFilter));

            if (checkStatus.GetValue() != "")
            {
                res = res.Where(e => e.CheckStatus == checkStatus);

                if (checkStatus == "ReAssign")
                    res = res.Where(e => e.BranchCode == branchCode);

                if (checkStatus == "ReferToOfficer")
                    res = res.Where(e => e.ClearingOfficer == clearingOfficer);
            }


            if (amountFromFilter == amountToFilter)
            {
                res = res.Where(e => e.CheckAmount >= amountFromFilter.ToDouble(0)).OrderBy(e => e.CheckAmount); ;
            }
            else
            {
                res = res.Where(e => e.CheckAmount >= amountFromFilter.ToDouble(0) && e.CheckAmount <= amountToFilter.ToDouble(0)).OrderBy(e => e.CheckAmount); ;
            }

            if (dateFrom == dateTo)
            {
                res = res.Where(e => e.EffectivityDate.Date >= dateFrom.ToDate().Date);
            }
            else
            {
                res = res.Where(e => e.EffectivityDate.Date >= dateFrom.ToDate().Date && e.EffectivityDate.Date <= dateTo.ToDate().Date);
            }


            var inwardClearingChequeDetailsList = res.ToList();
            string result = "";
            var skip = inwardClearingChequeDetailsList.FindIndex(new InwardClearingChequeDetailsSearch(chequeImageLinkedKey).StartsWith);
            if ((skip + 1) < inwardClearingChequeDetailsList.Count)
                return inwardClearingChequeDetailsList[(skip + 1)].ChequeImageLinkedKey;

            return result;

            //var inwardClearingChequeDetails = _context.InwardClearingChequeDetailsModel.
            //                                    Where(e => e.AccountNumber.StartsWith(filter));

            //if (!checkStatus.IsNull())
            //    inwardClearingChequeDetails = _context.InwardClearingChequeDetailsModel.
            //                                    Where(e => e.CheckStatus == checkStatus
            //                                        && e.AccountNumber.StartsWith(filter));

            //if (!dateFrom.IsNull() && !dateTo.IsNull())
            //    inwardClearingChequeDetails = _context.InwardClearingChequeDetailsModel.
            //                        Where(e => e.CheckStatus == checkStatus
            //                            && e.AccountNumber.StartsWith(filter)
            //                            && (e.EffectivityDate.Date >= dateFrom.ToDate().Date && e.EffectivityDate.Date <= dateTo.ToDate().Date)
            //                        );


            //if (checkStatus == "ReAssign")
            //    inwardClearingChequeDetails = inwardClearingChequeDetails.Where(e => e.BranchCode == branchCode);

            //if (checkStatus == "ReferToOfficer")
            //    inwardClearingChequeDetails = inwardClearingChequeDetails.Where(e => e.ClearingOfficer == clearingOfficer);

            //var inwardClearingChequeDetailsList = inwardClearingChequeDetails.ToList();
            //string result = "";
            //var skip = inwardClearingChequeDetailsList.FindIndex(new InwardClearingChequeDetailsSearch(chequeImageLinkedKey).StartsWith);
            //if ((skip + 1 ) < inwardClearingChequeDetailsList.Count)
            //        return inwardClearingChequeDetailsList[(skip + 1)].ChequeImageLinkedKey;

            //return result;
        }

        public string GetBranchEmail(string sBranchCode)
        {
            var branchModel = _dBContext.BranchModels.SingleOrDefault(e => e.BranchCode == sBranchCode && e.Isdeleted == false);
            if (!branchModel.IsNull())
                return branchModel.BranchEmail.GetValue();

            return "";
        }

        public string GetEmailBody(string sEmailFor)
        {
            var emailTemplateModel = _dBContext.EmailTemplateModels.SingleOrDefault(e => e.EmailFor == sEmailFor);
            if (!emailTemplateModel.IsNull())
                return emailTemplateModel.EmailBody.GetValue();

            return "Please set up email template";
        }

        public string GetEmailSubject(string sEmailFor)
        {
            var emailTemplateModel = _dBContext.EmailTemplateModels.SingleOrDefault(e => e.EmailFor == sEmailFor);
            if (!emailTemplateModel.IsNull())
                return emailTemplateModel.EmailSubjest.GetValue();

            return "Please set up email template";
        }

        public string GetBranchCodeViaBRSTN(string sAcctNo)
        {
            try
            {
                var sBRSTN = _dBContext.InwardClearingChequeDetailsModels.First(d => d.AccountNumber == sAcctNo);
                if (!sBRSTN.IsNull())
                {
                    var sBranchCode = _dBContext.BranchModels.First(e => e.BranchBrstn == sBRSTN.Brstn && e.Isdeleted == false);
                    if (!sBranchCode.IsNull())
                        return sBranchCode.BranchCode.GetValue();
                }
            }
            catch(Exception ex)
            {
                string m = ex.Message.ToString();
                return m;
            }
            return "";
        }

        public bool GetBCPMode(string sBranchCode)
        {
            var branchModel = _dBContext.BranchModelAuxes.FirstOrDefault(e => e.BranchCode == sBranchCode);
            if (!branchModel.IsNull())
                return branchModel.BranchBcp;

            return false;
        }
        public string GetUserLimits(string sUserID)
        {
            try
            {
                var res = _dBContext.UserAmountLimitModels.SingleOrDefault(e => e.UserId == sUserID && e.Isdeleted == false);
                if (!res.IsNull())
                    return res.AmountLimitId.GetValue();
            }
            catch(Exception ex)
            {
                string m = ex.Message;
                return m;
            }

            return "";
        }
        public string GetUserAllowedAmount(string sCode)
        {
            var res = _dBContext.AmountLimitsModels.SingleOrDefault(e => e.AmountLimitsCode == sCode && e.Isdeleted == false);
            if (!res.IsNull())
                return res.MaxAmountLimit.GetValue();

            return "";
        }

        public string GetUserAllowedAction(string sCode)
        {
            var res = _dBContext.AmountLimitsModels.SingleOrDefault(e => e.AmountLimitsCode == sCode && e.Isdeleted == false);
            if (!res.IsNull())
                return res.AllowedAction.GetValue();

            return "";
        }
        public void SaveImage(Stream fileStream, string imagePath, string imageFileName)
        {
            string path = Path.Combine(GetImageFolder(), imagePath);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filePath = Path.Combine(path, imageFileName);

            Bitmap.FromStream(fileStream).Save(filePath, ImageFormat.Png);
        }
        public IQueryable<RejectReasonModel> GetRejectReasonList()
        {
            var listOfReasons = _dBContext.RejectReasonModels.Where(e => e.Isdeleted == false);
            if (!listOfReasons.IsNull())
            {
                return listOfReasons;
            }
            return Enumerable.Empty<RejectReasonModel>().AsQueryable();
        }
        public IQueryable<UserModel> GetUserListByUserType(string sUserType, string sUserId = "")
        {
            var userModel = _dBContext.UserModels.Where(e => e.UserType == sUserType && e.Isdeleted == false && e.Isdisabled == false);
            if (!userModel.IsNull())
                return sUserId == "" ? userModel : userModel.Where(u => u.UserId != sUserId);

            return Enumerable.Empty<UserModel>().AsQueryable();
        }

        public byte[] ReadFileAsBytes(string filePath)
        {
            byte[] fileBytes = null;

            try
            {
                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read the contents of the file as bytes
                    fileBytes = File.ReadAllBytes(filePath);
                }
                else
                {
                    // Handle the case where the file does not exist
                    throw new FileNotFoundException("File not found", filePath);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during file reading
                Console.WriteLine("Error reading file: " + ex.Message);
                // You can choose to throw the exception here or handle it differently
            }

            return fileBytes;
        }
        #endregion

        #region Config
        private AppDbContext SetContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString("BOCCIVSContext"));
            return new AppDbContext(optionsBuilder.Options);
        }
        private string GetConnectionString(string sConnStr)
        {
            string r = "";
            if (sConnStr == "BOCCIVSContext")
                r = _config.GetConnectionString("BOCCIVSContext").Decrypt();

            //if (sConnStr == "PRDSVSConn")
            //    r = _config.GetConnectionString("PRDSVSConn");

            return r;
        }
        public string GetDomainName()
        {
            return _config["DomainName"].ToString();
        }
        public string GetFileOrDB()
        {
            return _config["FileOrDB"];
        }
        public string GetCheckImages()
        {
            return _config["CheckImages"];
        }
        public string GetImageFolder()
        {
            return _config["ImageFolder"];
        }
        public string GetDBAS400()
        {
            return _config["DBAS400"].ToString();
        }
        public string GetDBAS400DBServer()
        {
            return _config["DBAS400:DBServer"].ToString().Decrypt();
        }
        public string GetDBAS400Database()
        {
            return _config["DBAS400:Database"].ToString().Decrypt();
        }
        public string GetDBAS400UserName()
        {
            return _config["DBAS400:UserName"].ToString().Decrypt();
        }
        public string GetDBAS400Password()
        {
            return _config["DBAS400:Password"].ToString().Decrypt();
        }
        public string[] GetPrefixBack()
        {
            return _config.GetSection("ChequeFileName:PrefixBack").Get<string[]>();
        }
        public string GetChequeFileNameFormatLength()
        {
            return _config.GetSection("ChequeFileName:FormatLength").Get<string>();
        }
        public string GetChequeFileNameFormat()
        {
            return _config.GetSection("ChequeFileName:Format").Get<string>();
        }
        public string[] GetPrefixFront()
        {
            return _config.GetSection("ChequeFileName:PrefixFront").Get<string[]>();
        }
        public string[] GetImageContentTypeContentTypeF()
        {
            return _config.GetSection("ImageContentType:ContentTypeF").Get<string[]>();
        }
        public string[] GetImageContentTypeContentTypeR()
        {
            return _config.GetSection("ImageContentType:ContentTypeR").Get<string[]>();
        }

        #endregion
        #region Message
        public ReturnGenericStatus MsgError(string ErrMsg)
        {
            var status = new ReturnGenericStatus();
            status.StatusCode = "01";
            status.StatusMessage = ErrMsg;
            return status;
        }
        public ReturnGenericStatus MsgSuccess(string SucMsg)
        {
            var status = new ReturnGenericStatus();
            status.StatusCode = "00";
            status.StatusMessage = SucMsg;
            return status;
        }
        #endregion

        #region  Variable

        public enum MsgStatus
        {
            Succcess,
            Error,
            Info,
            Warning
        }
        #endregion
    }
}
 
    
