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
    public class UserTypeRepositories : IUserTypeRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;

        public UserTypeRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }
        public async Task<PaginatedOutput<UserTypeModel>> GetUserTypeList(string sKey = "", int page = 1)
        {
            try
            {
                IQueryable<UserTypeModel> UserTypeModels = _dBContext.UserTypeModels.OrderBy(b => b.UserTypeDesc);
                if (string.IsNullOrEmpty(sKey))
                    UserTypeModels = _dBContext.UserTypeModels.OrderBy(b => b.UserTypeDesc);
                else
                    UserTypeModels = _dBContext.UserTypeModels.Where(e => e.UserTypeDesc.Contains(sKey)).OrderBy(b => b.UserTypeDesc);

                PaginatedList<UserTypeModel> Paginate = await PaginatedList<UserTypeModel>.CreateAsync(UserTypeModels, page);
                var data = new PaginatedOutput<UserTypeModel>(Paginate);

                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<UserTypeModel>> GetUserType(int? id)
        {
            ReturnGenericStatus status = new();
            UserTypeModel Data = new();
            try
            {
                if (id == null)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    Data = await _dBContext.UserTypeModels.FirstOrDefaultAsync(m => m.Id == id);
                    if (Data == null)
                        status = _commonClass.MsgError("No data found");
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            ReturnGenericData<UserTypeModel> r = new()
            {
                StatusCode = status.StatusCode,
                StatusMessage = status.StatusMessage,
                Data = Data,
            };
            return r;
        }
        public async Task<ReturnGenericStatus> SaveUserType(UserTypeModel data)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! User type was successfully saved");
            try
            {
                if (data.Id == 0)
                {
                    if (!UserTypeModelExists(data.UserTypeCode))
                    {
                        _dBContext.Add(data);
                        await _dBContext.SaveChangesAsync();
                    }
                    else
                    {
                        status = _commonClass.MsgError("UserType Code Already Exist. Please Change.");
                    }
                }
                else
                {
                    if (UserTypeModelExists(data.UserTypeCode, data.Id))
                    {
                        status = _commonClass.MsgError("UserType Code Already Exist. Please Change.");
                    }
                    else
                    {
                        var UserTypeModel = await _dBContext.UserTypeModels.FindAsync(data.Id);
                        if (UserTypeModel == null)
                            status = _commonClass.MsgError("No data found");
                        else if (UserTypeModel.Isdeleted)
                            status = _commonClass.MsgError("No data found");
                        else
                        {
                            UserTypeModel.UserTypeCode = data.UserTypeCode;
                            UserTypeModel.UserTypeDesc = data.UserTypeDesc;
                            try
                            {
                                _dBContext.Update(UserTypeModel);
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
        public async Task<ReturnGenericStatus> DeleteUserType(int id)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Data successfully deleted");
            try
            {

                var UserTypeModel = await _dBContext.UserTypeModels.FindAsync(id);
                if (UserTypeModel == null)
                    status = _commonClass.MsgError("No data found");
                else if (UserTypeModel.Isdeleted)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    UserTypeModel.Isdeleted = true;
                    _dBContext.Update(UserTypeModel);
                    await _dBContext.SaveChangesAsync();
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintUserType()
        {
            ReturnGenericData<ReturnDownloadPDF> status = new ReturnGenericData<ReturnDownloadPDF>();
            try
            {
                List<UserTypeModel> ReportData = await _dBContext.UserTypeModels.ToListAsync();
                Dictionary<int, List<string>> DataList = ReportData
                                                       .ToDictionary(x => x.Id, x => (string.Format("{0}|{1}|{2}",
                                                                                     x.UserTypeCode, x.UserTypeDesc, x.Isdeleted)
                                                                                     .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                        );


                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userName = userClaims.DisplayName;

                PDFReport pDFReport = new PDFReport();
                string TitleHeader = "User Type List Report";
                List<int> widthList = new List<int> { 110, 220, 150 };
                List<string> Header = "User Type Code|User Type Desc|Deleted"
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                byte[] data = pDFReport.GeneratePDF(
                                          DataList,
                                          "UserTypeReport", userName, userId,
                                          TitleHeader,
                                          Header,
                                          widthList
                                          );
                string base64String = Convert.ToBase64String(data);
                string fileName = string.Format("UserTypeReport{0}.pdf", DateTime.Now.ToString("MMddyyyy"));

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
        private bool UserTypeModelExists(string UserTypeCode, int? id = null)
        {
            var ICount = _dBContext.UserTypeModels.Where(e => e.UserTypeCode == UserTypeCode && e.Isdeleted == false && (id ?? 0) != e.Id).Count();
            return ICount > 0;
        }

    }
}
