using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Infrastructure.Repositories
{
    public class UserRepositories : IUserRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;

        public UserRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }
        public async Task<PaginatedOutput<UserModel>> GetUserList(string sKey = "", string key = "", int page = 1)
        {
            if (key.ToLower().Contains("disabled"))
                key = "d1";
            else if (key.ToLower().Contains("deactivated"))
                key = "d2";


            IQueryable<UserModel> userModels = _dBContext.UserModels;
            switch ((key ?? "").ToLower())
            {
                case "d1":
                    userModels = _dBContext.UserModels.Where(e => e.Isdisabled == true);
                    break;
                case "d2":
                    userModels = _dBContext.UserModels.Where(e => e.Isdeleted == true);
                    break;
            }

            switch (sKey)
            {
                case "":
                    break;
                default:
                    userModels = userModels.Where(e => e.UserId.StartsWith(sKey));
                    break;
            }


            PaginatedList<UserModel> Paginate = await PaginatedList<UserModel>.CreateAsync(userModels, page);
            var data = new PaginatedOutput<UserModel>(Paginate);
            foreach (UserModel userModel in data.Data)
            {
                DisplayUserDetails(userModel);
            }

            return data;
        }
        public async Task<ReturnGenericData<UserModel>> GetUser(int? id)
        {
            ReturnGenericStatus status = new();
            UserModel Data = new();
            try
            {
                if (id == null)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    Data = await _dBContext.UserModels.FirstOrDefaultAsync(m => m.Id == id);
                    if (Data == null)
                        status = _commonClass.MsgError("No data found");
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            ReturnGenericData<UserModel> r = new()
            {
                StatusCode = status.StatusCode,
                StatusMessage = status.StatusMessage,
                Data = Data,
            };
            return r;
        }
        public async Task<ReturnGenericStatus> SaveUser(UserModel data)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! User was successfully saved");
            try
            {
                var userClaims = _userClaims.GetClaims();
                UserModel userModel = new();
                string userId = userClaims.UserID;
                string logMessage = string.Empty;
                if (data.UserDisplayName == null)
                {
                    var UserDisplayName = _commonClass.GetADCredInfo(data.UserId);
                    if (UserDisplayName != null) data.UserDisplayName = UserDisplayName.DisplayName;
                    else data.UserDisplayName = "";
                }

                if (data.UserType == null)
                    data.UserType = "0";

                if (data.BranchOfAssignment == null)
                    data.BranchOfAssignment = "";

                if (data.Id == 0)
                {
                    if (!UserModelExists(data.UserId))
                    {
                        logMessage = string.Format("{0} Enroll User [{1}]", userId, data.UserId);
                        userModel = data;
                        data.LastLoginSession = "";
                        _dBContext.Add(data);
                        await _dBContext.SaveChangesAsync();
                    }
                    else
                    {
                        status = _commonClass.MsgError("UserId Already Enrolled.");
                    }
                }
                else
                {
                    if (UserModelExists(data.UserId, data.Id))
                    {
                        status = _commonClass.MsgError("UserId Already Enrolled.");
                    }
                    else
                    {
                        var UserModel = await _dBContext.UserModels.FindAsync(data.Id);
                        userModel = UserModel;
                        if (UserModel == null)
                            status = _commonClass.MsgError("No data found");
                        else if (UserModel.Isdeleted)
                            status = _commonClass.MsgError("This user is already deleted");
                        else
                        {
                            UserModel.UserId = data.UserId;
                            UserModel.EmployeeNumber = data.EmployeeNumber;
                            UserModel.UserType = data.UserType;
                            UserModel.BranchOfAssignment = data.BranchOfAssignment;
                            try
                            {
                                logMessage = string.Format("{0} Update User [{1}]", userId, UserModel.UserId);
                                _dBContext.Update(UserModel);
                                await _dBContext.SaveChangesAsync();
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                throw;
                            }
                        }
                    }
                }

                if(status.StatusCode == "00")
                {
                    string messageDetails = string.Format("DisplayName : {0} UserType : {1} Branch : {2}",
                            userModel.UserDisplayName, userModel.UserType, userModel.BranchOfAssignment);
                    logMessage = string.Format("{0} {1}", logMessage, messageDetails);

                    _commonClass.Log(logMessage);
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        public async Task<ReturnGenericStatus> DeleteUser(int id)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Data successfully deleted");
            try
            {

                var UserModel = await _dBContext.UserModels.FindAsync(id);
                if (UserModel == null)
                    status = _commonClass.MsgError("No data found");
                else if (UserModel.Isdeleted)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    UserModel.Isdeleted = true;
                    _dBContext.Update(UserModel);
                    await _dBContext.SaveChangesAsync();
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        public async Task<ReturnGenericStatus> Able(int id, bool mode)
        {
            ReturnGenericStatus status = new();
            try
            {
                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string messageDetails = "";


                UserModel userModel = await _dBContext.UserModels.FindAsync(id);
                if (userModel == null)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    userModel.Isdisabled = !mode;
                    _dBContext.Update(userModel);
                    await _dBContext.SaveChangesAsync();
                    
                    string logMessage = string.Format("{0} {1} User [{2}]", userId, (!mode ? "Disabled" : "Enabled"), userModel.UserId);
                    messageDetails = string.Format("DisplayName : {0} UserType : {1} Branch : {2}",
                            userModel.UserDisplayName, userModel.UserType, userModel.BranchOfAssignment);
                    logMessage = string.Format("{0} {1}", logMessage, messageDetails);

                    _commonClass.Log(logMessage);
                    status = _commonClass.MsgSuccess("Successful! Data successfully " + (string.Format("{0} {1}", userModel.UserDisplayName, !mode ? "Disabled" : "Enabled")));
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        public async Task<ReturnGenericStatus> Activate(int id, bool mode)
        {
            ReturnGenericStatus status = new();
            try
            {
                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string messageDetails = "";
                var userModel = await _dBContext.UserModels.FindAsync(id);
                if (userModel == null)
                    status = _commonClass.MsgError("No data found");
                else{
                    userModel.Isdeleted = !mode;
                    _dBContext.Update(userModel);
                    await _dBContext.SaveChangesAsync();

                    string logMessage = string.Format("{0} {1} User [{2}]", userId, !mode ? "Deactivate" : "Reactivate", userModel.UserId);
                    messageDetails = string.Format("DisplayName : {0} UserType : {1} Branch : {2}",
                            userModel.UserDisplayName, userModel.UserType, userModel.BranchOfAssignment);
                    logMessage = string.Format("{0} {1}", logMessage, messageDetails);

                    _commonClass.Log(logMessage);
                    status = _commonClass.MsgSuccess("Successful! Data successfully " + (string.Format("{0} {1}", userModel.UserDisplayName, !mode ? "Deactivate" : "Reactivate")));
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        public async Task<ReturnGenericDropdown> GetUserTypeList()
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            try
            {
                List<SelectListItem> result = await _dBContext.UserTypeModels.Select(x => new SelectListItem { Text = x.UserTypeDesc, Value = x.UserTypeCode, IsChecked = false }).OrderBy(u => u.Text).ToListAsync<SelectListItem>();
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
        public async Task<ReturnGenericDropdown> GetBranchOfAssignmentList()
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            try
            {
                List<SelectListItem> result = await _dBContext.BranchModels.Select(x => new SelectListItem { Text = x.BranchDesc, Value = x.BranchCode, IsChecked = false }).OrderBy(u => u.Text).ToListAsync<SelectListItem>();
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
        public async Task<ReturnGenericDropdown> GetGroupList()
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            try
            {
                List<SelectListItem> result = await _dBContext.GroupModels.Select(x => new SelectListItem { Text = x.GroupDesc, Value = x.GroupCode, IsChecked = false }).OrderBy(u => u.Text).ToListAsync<SelectListItem>();
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
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintUser(string key = "")
        {
            ReturnGenericData<ReturnDownloadPDF> status = new ReturnGenericData<ReturnDownloadPDF>();
            try
            {
                IQueryable<UserModel> userModels = _dBContext.UserModels;

                string mode = (key).ToLower().Contains("disabled") ? "d1" : key;
                mode = mode.ToLower().Contains("deactivated") ? "d2" : mode;

                switch (mode.ToLower())
                {
                    case "d1":
                        userModels = _dBContext.UserModels.Where(e => e.Isdisabled == true);
                        break;
                    case "d2":
                        userModels = _dBContext.UserModels.Where(e => e.Isdeleted == true);
                        break;
                    default:
                        userModels = _dBContext.UserModels;
                        break;
                }
                List<UserModel> ReportData = await userModels.ToListAsync();
                Dictionary<int, List<string>> DataList = ReportData
                                                       .ToDictionary(x => x.Id, x => (string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                                                                                        string.Format("{0}\n({1})", x.UserId, x.EmployeeNumber),
                                                                                        x.UserDisplayName,
                                                                                        string.Format("{0}\n({1})", x.UserTypeDesc.GetValue(), x.GroupingDesc),
                                                                                        x.BranchOfAssignmentDesc,
                                                                                        string.Format("{0}\n{1}", x.Isdisabled, x.Isdeleted),
                                                                                        x.LastLoginDate)
                                                                                     .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                        );

                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userName = userClaims.DisplayName;

                PDFReport pDFReport = new PDFReport();
                string TitleHeader = (key == "" ? "User ID Matrix Report" : string.Format("{0} User ID", key));
                List<int> widthList = new List<int> { 100, 220, 200, 250, 100, 100 };
                List<string> Header = "User ID\nEmployee Number|User Name|User Type\nUser Group|Branch Of Assignment|Disabled\nDeleted|Last Login Date"
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                byte[] data;
                if ((key ?? "") == "")
                {
                    string TitleHeaderP2 = "User Access Rights";
                    List<int> widthListP2 = new List<int> { 150, 400 };
                    int ColumnCustomHeightP2 = 1;
                    List<string> HeaderP2 = "Group Description|Menu Access"
                                    .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                    Dictionary<int, List<string>> DataListP2 = _dBContext.GroupAccessModels.ToList().ToDictionary(x => x.Id, x => (string.Format("{0}|{1}",
                                                                                          _commonClass.GetGroupDesc(x.GroupId), _commonClass.GetMenuIDsDesc(x.MenuIds).Replace("|", "\n"))
                                                                                         .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                                                          );


                    string TitleHeaderP3 = "Menu Access";
                    List<int> widthListP3 = new List<int> { 300, 400 };
                    int ColumnCustomHeightP3 = 1;
                    List<string> HeaderP3 = "Menu|Sub Menu"
                                    .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                    Dictionary<int, List<string>> DataListP3 = _dBContext.MenuModels.Where(e => e.RootMenu == true).ToList().ToDictionary(x => x.Id, x => (string.Format("{0}|{1}",
                                                                                          x.MenuDesc, _commonClass.GetMenuIDsDesc(x.SubMenus ?? "").Replace("|", "\n"))
                                                                                         .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                                                         );


                    data = pDFReport.GeneratePDF(
                                              DataList,
                                              "UserReport", userName, userId,
                                              TitleHeader,
                                              Header,
                                              widthList,
                                              0,
                                              10,
                                              TitleHeaderP2,
                                              HeaderP2,
                                              DataListP2,
                                              10,
                                              widthListP2,
                                              ColumnCustomHeightP2,
                                              TitleHeaderP3,
                                              HeaderP3,
                                              DataListP3,
                                              20,
                                              widthListP3,
                                              ColumnCustomHeightP3
                                              );
                }
                else
                {

                    data = pDFReport.GeneratePDF(
                                              DataList,
                                              "UserReport", userName, userId,
                                              TitleHeader,
                                              Header,
                                              widthList
                                              );
                }
                string base64String = Convert.ToBase64String(data);
                string fileName = string.Format("UserReport{0}.pdf", DateTime.Now.ToString("MMddyyyy"));

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
        private bool UserModelExists(string UserId, int? id = null)
        {
            var ICount = _dBContext.UserModels.Where(e => e.UserId == UserId && e.Isdeleted == false && (id ?? 0) != e.Id).Count();
            return ICount > 0;
        }
        private void DisplayUserDetails(UserModel userModel)
        {
            userModel.UserDisplayName = userModel.UserDisplayName;
            userModel.GroupCode = _commonClass.GetGroupIDbyUserType(userModel.UserType ?? "");
            userModel.GroupingDesc = _commonClass.GetGroupDesc(userModel.GroupCode);
            userModel.UserTypeDesc = _commonClass.GetUserTypeDesc(userModel.UserType ?? "");
            userModel.BranchOfAssignmentDesc = _commonClass.GetBranchOfAssignmentDesc(userModel.BranchOfAssignment ?? "");
            userModel.LastLoginDate = userModel.LastLoginDate;
        }

    }
}
