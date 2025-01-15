using Application.Interfaces;
using Application.Models;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Infrastructure.Repositories
{
    public class MenuRepositories : IMenuRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;

        public MenuRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass, IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }
        public async Task<PaginatedOutput<MenuModel>> GetMenuList(int page = 1)
        {
            try
            {
                IQueryable<MenuModel> MenuModels = _dBContext.MenuModels.OrderBy(b => b.MenuCode);


                PaginatedList<MenuModel> Paginate = await PaginatedList<MenuModel>.CreateAsync(MenuModels, page);
                var data = new PaginatedOutput<MenuModel>(Paginate);

                foreach (var x in data.Data)
                {
                    x.SubMenusDesc = _commonClass.GetMenuIDsDesc(x.SubMenus ?? "");
                }
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<ReturnGenericData<MenuModel>> GetMenu(int? id)
        {
            ReturnGenericStatus status = new();
            MenuModel Data = new();
            try
            {
                if (id == null)
                    status = _commonClass.MsgError("No data found");
                else
                {
                    Data = await _dBContext.MenuModels.FirstOrDefaultAsync(m => m.Id == id);
                    if (Data == null)
                        status = _commonClass.MsgError("No data found");
                    else
                    {
                        //Data.ActionMethodParam = ActionParamDesc(Data.ActionMethodParam ?? "");
                        Data.SelectedSubMenus = new string[] { };
                        Data.SubMenusDesc = _commonClass.GetMenuIDsDesc(Data.SubMenus ?? "");
                    }
                }
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            ReturnGenericData<MenuModel> r = new()
            {
                StatusCode = status.StatusCode,
                StatusMessage = status.StatusMessage,
                Data = Data,
            };
            return r;
        }
        public async Task<ReturnGenericStatus> SaveMenu(MenuModel data)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Menu was successfully saved");
            try
            {
                if (data.Id == 0)
                {
                    if (!MenuModelExists(data.MenuCode))
                    {
                        data.SubMenus = string.Join(",", data.SelectedSubMenus);
                        _dBContext.Add(data);
                        await _dBContext.SaveChangesAsync();
                    }
                    else
                    {
                        status = _commonClass.MsgSuccess("Menu Code Already Exist. Please Change.");
                    }
                }
                else
                {
                    if (MenuModelExists(data.MenuCode, data.Id))
                    {
                        status = _commonClass.MsgSuccess("Menu Code Already Exist. Please Change.");
                    }
                    else
                    {
                        var MenuModel = await _dBContext.MenuModels.FindAsync(data.Id);
                        if (MenuModel == null)
                            status = _commonClass.MsgError("No data found");
                        else if (MenuModel.Isdeleted)
                            status = _commonClass.MsgError("No data found");
                        else
                        {
                            MenuModel.MenuCode = data.MenuCode;
                            MenuModel.MenuDesc = data.MenuDesc;
                            MenuModel.Controller = data.Controller;
                            MenuModel.ActionMethod = data.ActionMethod;
                            MenuModel.ActionMethodParam = data.ActionMethodParam;
                            MenuModel.RootMenu = data.RootMenu;
                            MenuModel.SubMenus = string.Join(",", data.SelectedSubMenus);
                            try
                            {
                                _dBContext.Update(MenuModel);
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
        public async Task<ReturnGenericStatus> DeleteMenu(int id)
        {
            ReturnGenericStatus status = new();
            status = _commonClass.MsgSuccess("Successful! Data successfully deleted");
            try
            {
                var MenuModel = await _dBContext.MenuModels.FindAsync(id);
                if (MenuModel == null)
                    status = _commonClass.MsgError("No data found");
                else if (MenuModel.Isdeleted)
                    status = _commonClass.MsgError("No data found");
                else
                {

                }
                MenuModel.Isdeleted = true;
                _dBContext.Update(MenuModel);
                await _dBContext.SaveChangesAsync();
            }
            catch (Exception err)
            {
                status = _commonClass.MsgError(err.Message);
            }
            return status;
        }
        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintMenu()
        {
            ReturnGenericData<ReturnDownloadPDF> status = new ReturnGenericData<ReturnDownloadPDF>();
            try
            {
                List<MenuModel> chekReportDetail = _dBContext.MenuModels.Where(e => e.RootMenu == true).ToList();
                Dictionary<int, List<string>> DataList = chekReportDetail
                                                       .ToDictionary(x => x.Id, x => (string.Format("{0}|{1}|{2}|{3}",
                                                                                     x.MenuCode, x.MenuDesc, _commonClass.GetMenuIDsDesc(x.SubMenus ?? "").Replace("|", "\n"), x.Isdeleted)
                                                                                     .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                        );

                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userName = userClaims.DisplayName;

                PDFReport pDFReport = new PDFReport();
                string TitleHeader = "Menu List Report";
                List<int> widthList = new List<int> { 100, 250, 300, 200 };
                List<string> Header = "Menu Code|Menu Desc|Sub Menu|Deleted"
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                int ColumnCustomHeight = 2, lineNoLimit = 20;
                byte[] data = pDFReport.GeneratePDF(
                                          DataList,
                                          "MenuListReport", userName, userId,
                                          TitleHeader,
                                          Header,
                                          widthList,
                                          ColumnCustomHeight,
                                          lineNoLimit
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
        public async Task<ReturnGenericDropdown> GetSubmenuList()
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            try
            {
                List<SelectListItem> result = await _dBContext.MenuModels.Where(e => e.RootMenu == false && e.Isdeleted == false)
                    .Select(e => new SelectListItem
                    {
                        Text = e.MenuDesc,
                        Value = e.MenuCode
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
        public ReturnGenericDropdown GetActionMethodList()
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            try
            {
                List<SelectListItem> result = new List<SelectListItem>
                                        {
                                            new SelectListItem { Text = "Index", Value = "Index", IsChecked = false},
                                            new SelectListItem { Text = "Create", Value = "Create", IsChecked = false},
                                            new SelectListItem { Text = "Details", Value = "Details", IsChecked = false},
                                            new SelectListItem { Text = "Report", Value = "Report", IsChecked = false},
                                            new SelectListItem { Text = "List", Value = "List", IsChecked = false}
                                        };
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
        public ReturnGenericDropdown GetActionMethodParamList()
        {
            ReturnGenericStatus status = new ReturnGenericStatus();
            ReturnGenericDropdown x = new ReturnGenericDropdown();
            try
            {
                List<SelectListItem> result = MakeListActionParams();
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
        private static List<SelectListItem> MakeListActionParams()
        {
            return new List<SelectListItem>
                    {
                        new SelectListItem { Text="Status Accept", Value="Accept" , IsChecked = false},
                        new SelectListItem { Text="Deactivated User", Value="Deactivated" , IsChecked = false},
                        new SelectListItem { Text="Disabled User", Value="Disabled" , IsChecked = false},
                        new SelectListItem { Text="LargeAmount^99999 Report", Value="LargeAmount^99999" , IsChecked = false},
                        new SelectListItem { Text="MCImages Report", Value="MCImages" , IsChecked = false},
                        new SelectListItem { Text="Open", Value="Open" , IsChecked = false},
                        new SelectListItem { Text="For Refer to Branch", Value="RB" , IsChecked = false},
                        new SelectListItem { Text="Status Reject", Value="Reject" , IsChecked = false},
                        new SelectListItem { Text="Status ReAssign", Value="ReAssign" , IsChecked = false},
                        new SelectListItem { Text="Status Next Level Approver", Value="ReferToOfficer" , IsChecked = false},
                        new SelectListItem { Text="For Refer to Next Level Approver", Value="RO" , IsChecked = false},
                        new SelectListItem { Text="Status Upload", Value="Upload" , IsChecked = false},
                        new SelectListItem { Text="With Technicalities Report", Value="WithTechnicalities" , IsChecked = false},
                        new SelectListItem { Text="Closed Account", Value="AcctClsd" , IsChecked = false},
                        new SelectListItem { Text="Dormant Account", Value="AcctDrmt" , IsChecked = false},
                        new SelectListItem { Text="Post No Debit Account", Value="AcctPnD" , IsChecked = false},
                    };
        }
        private bool MenuModelExists(string MenuCode, int? id = null)
        {
            var ICount = _dBContext.MenuModels.Where(e => e.MenuCode == MenuCode && e.Isdeleted == false && (id ?? 0) != e.Id).Count();
            return ICount > 0;
        }
    }
}
