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
    public class UserAmountLimitRepositories : IUserAmountLimitRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;

        public UserAmountLimitRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }
        public async Task<PaginatedOutput<UserAmountLimitModel>> GetUserAmountLimitList(int page = 1)
        {
            try
            {
                IQueryable<UserAmountLimitModel> UserAmountLimitModels = _dBContext.UserAmountLimitModels.OrderBy(b => b.UserId);


                PaginatedList<UserAmountLimitModel> Paginate = await PaginatedList<UserAmountLimitModel>.CreateAsync(UserAmountLimitModels, page);
                var data = new PaginatedOutput<UserAmountLimitModel>(Paginate);

                foreach (var x in data.Data)
                {
                    x.UserDisplay = _commonClass.GetUserDisplayName(x.UserId);
                    x.AmountLimitDesc = GetAccessDesc(x.AmountLimitId);
                    x.AmountLimitDescMore = GetAccessDesc(x.AmountLimitId,true);
                }
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<UserAmountLimitModel>> GetUserAmountLimit(int? id)
        {
            ReturnGenericStatus status = new();
            UserAmountLimitModel Data = new();
            try
            {
                if (id == null)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    Data = await _dBContext.UserAmountLimitModels.FirstOrDefaultAsync(m => m.Id == id);
                    if (Data == null)
                        status = _commonClass.MsgError("No data found");
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            ReturnGenericData<UserAmountLimitModel> r = new()
            {
                StatusCode = status.StatusCode,
                StatusMessage = status.StatusMessage,
                Data = Data,
            };
            return r;
        }
        public async Task<ReturnGenericStatus> SaveUserAmountLimit(UserAmountLimitModel data)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! User amount limit was successfully saved");
            try
            {
                if (data.Id == 0)
                {
                    if (!UserAmountLimitModelExists(data.UserId))
                    {
                        data.Isdeleted = false;
                        _dBContext.Add(data);
                        await _dBContext.SaveChangesAsync();
                    }
                    else
                    {
                        status = _commonClass.MsgError("User amount limit code Already Exist. Please Change.");
                    }
                }
                else
                {
                    if (UserAmountLimitModelExists(data.UserId, data.Id))
                    {
                        status = _commonClass.MsgError("User amount limit code Already Exist. Please Change.");
                    }
                    else
                    {
                        var UserAmountLimitModel = await _dBContext.UserAmountLimitModels.FindAsync(data.Id);
                        if (UserAmountLimitModel == null)
                            status = _commonClass.MsgError("No data found");
                        if (UserAmountLimitModel.Isdeleted ?? false)
                            status = _commonClass.MsgError("No data found");
                        UserAmountLimitModel.AmountLimitId = data.AmountLimitId;
                        try
                        {
                            _dBContext.Update(UserAmountLimitModel);
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
        public async Task<ReturnGenericStatus> DeleteUserAmountLimit(int id)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Data successfully deleted");
            try
            {
                var UserAmountLimitModel = await _dBContext.UserAmountLimitModels.FindAsync(id);
                if (UserAmountLimitModel == null)
                    status = _commonClass.MsgError("No data found");
                if (UserAmountLimitModel.Isdeleted ?? false)
                    status = _commonClass.MsgError("No data found");

                UserAmountLimitModel.Isdeleted = true;
                _dBContext.Update(UserAmountLimitModel);
                await _dBContext.SaveChangesAsync();
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        private bool UserAmountLimitModelExists(string UserId, int? id = null)
        {
            var ICount = _dBContext.UserAmountLimitModels.Where(e => e.UserId == UserId && e.Isdeleted == false && (id ?? 0) != e.Id).Count();
            return ICount > 0;
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintUserAmountLimit()
        {
            ReturnGenericData<ReturnDownloadPDF> status = new ReturnGenericData<ReturnDownloadPDF>();
            try
            {
                List<UserAmountLimitModel> ReportData = await _dBContext.UserAmountLimitModels.ToListAsync();
                foreach (var x in ReportData)
                {
                    Dictionary<string, string> limits = _commonClass.GetUserLimitsDetails(x.AmountLimitId);
                    x.LimitsDesc = limits["LimitsDesc"];
                    x.LimitAmount = limits["LimitAmount"];
                    x.LimitAction = limits["LimitAction"];
                    x.UserDisplay = _commonClass.GetUserDisplayName(x.UserId);
                    x.AmountLimitDesc = GetAccessDesc(x.AmountLimitId);
                }
                Dictionary<int, List<string>> DataList = ReportData
                                                       .ToDictionary(x => x.Id, x => (string.Format("{0}|{1}|{2}|{3}|{4}",
                                                                                     x.UserId, x.LimitsDesc, x.LimitAction, x.LimitAction, x.Isdeleted)
                                                                                     .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                        );

                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userName = userClaims.DisplayName;

                PDFReport pDFReport = new PDFReport();
                string TitleHeader = "User Amount Limits Report";
                List<int> widthList = new List<int> { 150, 250, 150, 300, 200 };
                List<string> Header = "User Id|Amount Limits Desc|Max Amount Limit|Acctions|Deleted"
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                byte[] data = pDFReport.GeneratePDF(
                                          DataList,
                                          "UserAmountLimitsReport", userName, userId,
                                          TitleHeader,
                                          Header,
                                          widthList
                                          );
                string base64String = Convert.ToBase64String(data);
                string fileName = string.Format("UserAmountLimitsReport{0}.pdf", DateTime.Now.ToString("MMddyyyy"));

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
        public async Task<ReturnGenericDropdown> GetUserTypeList()
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            try
            {
                List<SelectListItem> UserTypeList = await _dBContext.UserTypeModels.Where(u => u.UserTypeDesc.Contains("clearing"))
                   .Select(e => new SelectListItem
                   {
                       Text = e.UserTypeDesc,
                       Value = e.UserTypeCode
                   }
                   ).ToListAsync<SelectListItem>();

                status = _commonClass.MsgSuccess("");
                x.Data = UserTypeList;
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            x.StatusMessage = status.StatusMessage;
            x.StatusCode = status.StatusCode;
            return x;
        }
        public async Task<ReturnGenericDropdown> GetUserList(string type)
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            try
            {

                List<SelectListItem> userList = await _dBContext.UserModels.Where(e => e.Isdeleted == false && e.UserType.Equals(type))
                    .Select(e => new SelectListItem
                    {
                        Text = e.UserDisplayName,
                        Value = e.UserId,
                        IsChecked = false,
                    }).ToListAsync<SelectListItem>();

                status = _commonClass.MsgSuccess("");
                x.Data = userList;
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            x.StatusMessage = status.StatusMessage;
            x.StatusCode = status.StatusCode;
            return x;
        }
        public async Task<ReturnGenericDropdown> GetAmountLimitList()
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            try
            {

                List<SelectListItem> result = await _dBContext.AmountLimitsModels.Where(e => e.Isdeleted == false)
                    .Select(e => new SelectListItem
                    {
                        Text = e.AmountLimitsDesc,
                        Value = e.AmountLimitsCode
                    }
                    ).ToListAsync<SelectListItem>();

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
        private string GetAccessDesc(string amountLimitID, bool showMore = false)
        {
            var limits = _commonClass.GetUserLimitsDetails(amountLimitID);

            if (showMore == false)
                return string.Format("{0}", limits["LimitsDesc"]);
            else
                return string.Format("{0}, Max Amount Limit:{1}. Allowed Action:{2}.", limits["LimitsDesc"], limits["LimitAmount"], limits["LimitAction"]);
        }
        //private void MakeViewBagList()
        //{
        //    var userTypeList = _dBContext.UserTypeModels.Where(u => u.UserTypeDesc.Contains("clearing"))
        //        .Select(e => new SelectListItem
        //        {
        //            Text = e.UserTypeDesc,
        //            Value = e.UserTypeCode
        //        }
        //        ).ToList();
        //    var UserTypeList = new SelectList(userTypeList, "Value", "Text");

        //    List<SelectListItem> viewBag = new List<SelectListItem>();

        //    var UserList = viewBag;

        //    var result = _dBContext.AmountLimitsModels.Where(e => e.Isdeleted == false)
        //        .Select(e => new SelectListItem
        //        {
        //            Text = e.AmountLimitsDesc,
        //            Value = e.AmountLimitsCode
        //        }
        //        ).ToList();

        //    var AmountLimitsList = new SelectList(result, "Value", "Text");
        //}
    }
}
