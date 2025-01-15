using Application.Interfaces;
using Application.Models;
using Azure;
using Infrastructure.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
using static Infrastructure.Common.GeneratePDF;

namespace Infrastructure.Repositories
{
    public class GroupRepositories : IGroupRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GeneratePDF _generatePDF;
        private readonly GetUserClaims _userClaims;
        public GroupRepositories(
            AppDbContext dBContext, 
            ADClass addClass, 
            CommonClass commonClass,
            GeneratePDF generatePDF,
            IHttpContextAccessor httpContextAccessor)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _generatePDF = generatePDF;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }

        #region PDF Region
        private static XTextFormatter xTextFormatter;
        private static PdfPage pdfPage;
        private static XRect xRect = new XRect();
        private static XGraphics xGraphics;
        private static readonly XFont xFontRegular6 = new XFont("Courier New", 6, XFontStyleEx.Regular);
        private static readonly XFont xFontRegular8 = new XFont("Courier New", 8, XFontStyleEx.Regular);
        private static readonly XFont xFontRegular10 = new XFont("Courier New", 10, XFontStyleEx.Regular);
        private static readonly XFont xFontRegular12 = new XFont("Courier New", 12, XFontStyleEx.Regular);
        private static readonly XFont xFontBold6 = new XFont("Courier New Bold", 6, XFontStyleEx.Bold);
        private static readonly XFont xFontBold8 = new XFont("Courier New Bold", 8, XFontStyleEx.Bold);
        private static readonly XFont xFontBold10 = new XFont("Courier New Bold", 10, XFontStyleEx.Bold);
        private static readonly XFont xFontBold12 = new XFont("Courier New Bold", 12, XFontStyleEx.Bold);
        private static readonly XFont xFontBold25 = new XFont("Courier New Bold", 25, XFontStyleEx.Bold);
        private static readonly XFont xFontBold35 = new XFont("Courier New Bold", 35, XFontStyleEx.Bold);
        //private static MemoryStream _Stream;// = new MemoryStream();
        private static readonly XFont xFontArialBold6 = new XFont("Arial Bold", 6, XFontStyleEx.Bold);
        private static readonly XFont xFontArialBold8 = new XFont("Arial Bold", 8, XFontStyleEx.Bold);
        private static XPen xPen = new XPen(XColors.Black, 1);
        private static XPen xPen2 = new XPen(XColors.Black, 2);
        private static XPen xPen3 = new XPen(XColors.Black, 3);
        private static XPen xPen4 = new XPen(XColors.Black, 4);

        private XFont GetFontType(FontType fontType)
        {
            XFont xFont = xFontRegular12;

            switch (fontType)
            {
                case FontType.xFontRegular6:
                    xFont = xFontRegular6;
                    break;

                case FontType.xFontRegular8:
                    xFont = xFontRegular8;
                    break;
                case FontType.xFontRegular10:
                    xFont = xFontRegular10;
                    break;
                case FontType.xFontRegular12:
                    xFont = xFontRegular12;
                    break;
                case FontType.xFontBold6:
                    xFont = xFontBold6;
                    break;
                case FontType.xFontBold8:
                    xFont = xFontBold8;
                    break;
                case FontType.xFontBold10:
                    xFont = xFontBold10;
                    break;
                case FontType.xFontBold12:
                    xFont = xFontBold12;
                    break;
                case FontType.xFontBold25:
                    xFont = xFontBold25;
                    break;
                case FontType.xFontBold35:
                    xFont = xFontBold35;
                    break;


                case FontType.xFontArialBold6:
                    xFont = xFontArialBold6;
                    break;
                case FontType.xFontArialBold8:
                    xFont = xFontArialBold8;
                    break;
            }
            return xFont;
        }

        private XPen GetPenWidth(PenWidth penWidth)
        {
            XPen _xPen = xPen;
            switch (penWidth)
            {
                case PenWidth.xPen:
                    _xPen = xPen;
                    break;
                case PenWidth.xPen2:
                    _xPen = xPen2;
                    break;
                case PenWidth.xPen3:
                    _xPen = xPen3;
                    break;
                case PenWidth.xPen4:
                    _xPen = xPen4;
                    break;
            }
            return _xPen;
        }

        private XParagraphAlignment GetParagraphAlignment(Alignment alignment)
        {
            XParagraphAlignment result = XParagraphAlignment.Default;

            if (alignment == Alignment.Center)
                result = XParagraphAlignment.Center;

            if (alignment == Alignment.Left)
                result = XParagraphAlignment.Left;

            if (alignment == Alignment.Right)
                result = XParagraphAlignment.Right;

            if (alignment == Alignment.Justify)
                result = XParagraphAlignment.Justify;

            return result;
        }

        public long StreamLength;

        public byte[] StreamByte;

        #endregion


        public async Task<PaginatedOutput<GroupModel>> GetGroup(int? page)
        {
            //var obj = new PaginatedOutput<GroupModel>();
            //var status = new ReturnGenericStatus();
            //obj.StatusCode = "01";
            try
            {
                PaginatedList<GroupModel> Paginate = await PaginatedList<GroupModel>.CreateAsync(_dBContext.GroupModels, page ?? 1);
                var data = new PaginatedOutput<GroupModel>(Paginate);

                return data;
                //var groupAccessModelList = await PaginatedList<GroupModel>.CreateAsync(_dBContext.GroupModels, page ?? 1);
                //obj.StatusCode = "00";
                //obj.StatusMessage = "SUCCESS";
                //obj.Data = groupAccessModelList;

            }
            catch (Exception ex)
            {
                return null;
                //status = _commonClass.MsgError(ex.Message);
                //obj.StatusMessage = status.StatusMessage;
                //obj.StatusCode = status.StatusCode;
            }
            //return obj;
        }

        public async Task<ReturnGenericData<ReturnGroup>> GetGroupDetails(int id)
        {
            var obj = new ReturnGenericData<ReturnGroup>();
            var status = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                var groupModel = await _dBContext.GroupModels
                                .Where(m => m.Id == id)
                                .Select(m => new ReturnGroup {
                                    Id = m.Id,
                                    GroupCode = m.GroupCode,
                                    GroupDesc = m.GroupDesc,
                                    Isdeleted = m.Isdeleted
                                    
                                })
                                .FirstOrDefaultAsync();

                if (groupModel == null)
                {
                    obj.StatusMessage = "Not found";
                }
                else
                {
                    groupModel.AccessAction = GroupAccessModelExists(groupModel.GroupCode) == true ? "Edit" : "Create";
                    groupModel.MemberAction = GroupMemberModelExists(groupModel.GroupCode) == true ? "Edit" : "Create";
                    obj.StatusCode = "00";
                    obj.StatusMessage = "SUCCESS";
                    obj.Data = groupModel;
                }
            }
            catch (Exception ex)
            {
                status = _commonClass.MsgError(ex.Message);
                obj.StatusMessage = status.StatusMessage;
                obj.StatusCode = status.StatusCode;
            }
            return obj;
        }

        public async Task<ReturnGenericStatus> GroupCreate(GroupModel value)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                if (!GroupModelExists(value.GroupCode))
                {
                    _dBContext.Add(value);
                    await _dBContext.SaveChangesAsync();
                    obj.StatusMessage = "SUCCESS";
                    obj.StatusCode = "00";
                }
                else
                {
                    var status = _commonClass.MsgError("Group Code Already Exist. Please Change.");
                    obj = status;
                }
            }
            catch (Exception ex)
            {
                var status = _commonClass.MsgError(ex.Message);
                obj = status;
            }
            return obj;
        }

        public async Task<ReturnGenericStatus> GroupEdit(int id, GroupModel value)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                if (!GroupModelExists(value.Id))
                {
                    var status = _commonClass.MsgError("Not found.");
                    obj = status;
                }
                else
                {
                    _dBContext.Update(value);
                    await _dBContext.SaveChangesAsync();
                    obj.StatusCode = "00";
                    obj.StatusMessage = "SUCCESS";
                }
                
            }
            catch (Exception ex)
            {
                var status = _commonClass.MsgError(ex.Message);
                obj = status;
            }
            return obj;
        }

        public async Task<ReturnGenericStatus> GroupDeleteConfirmed(int id)
        {
            var obj = new ReturnGenericStatus();
            obj.StatusCode = "01";
            try
            {
                var groupModel = await _dBContext.GroupModels.FindAsync(id);
                if (GroupMemberActive(groupModel.GroupCode))
                {
                    var status = _commonClass.MsgError("Group Code In Use. Cannot Delete.");
                    obj = status;
                }
                else
                {
                    if (groupModel.Isdeleted == false)
                        groupModel.Isdeleted = true;

                    DeleteGroupAccessAndMembers(groupModel.GroupCode);
                    _dBContext.Update(groupModel);
                    await _dBContext.SaveChangesAsync();
                    obj.StatusCode = "00";
                    obj.StatusMessage = "SUCCESS";
                }

            }
            catch (Exception ex)
            {
                var status = _commonClass.MsgError(ex.Message);
                obj = status;
            }
            return obj;
        }

        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintReport()
        {
            var obj = new ReturnGenericData<ReturnDownloadPDF>();
            try
            {
                List<GroupModel> ReportData = await _dBContext.GroupModels.ToListAsync();
                Dictionary<int, List<string>> DataList = ReportData
                                                       .ToDictionary(x => x.Id, x => (string.Format("{0}|{1}|{2}",
                                                                                     x.GroupCode, x.GroupDesc, x.Isdeleted)
                                                                                     .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                        );

                var userClaims = _userClaims.GetClaims();
                string userName = userClaims.DisplayName,
                       userId = userClaims.UserID; 

                PDFReport pDFReport = new PDFReport();
                string TitleHeader = "Group List Report";
                List<int> widthList = new List<int> { 100, 220, 200 };
                List<string> Header = "Group Code|Group Desc|Deleted"
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();

                string TitleHeaderP2 = "Group Access Rights";
                List<int> widthListP2 = new List<int> { 150, 400 };
                int ColumnCustomHeightP2 = 1;
                List<string> HeaderP2 = "Group Description|Menu Access"
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                Dictionary<int, List<string>> DataListP2 = _dBContext.GroupAccessModels.ToList().ToDictionary(x => x.Id, x => (string.Format("{0}|{1}",
                                                                                      _commonClass.GetGroupDesc(x.GroupId), _commonClass.GetMenuIDsDesc(x.MenuIds).Replace("|", "\n"))
                                                                                     .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList())
                                                        );


                string TitleHeaderP3 = "Group Members";
                List<int> widthListP3 = new List<int> { 150, 400 };
                int ColumnCustomHeightP3 = 1;
                List<string> HeaderP3 = "Group Description|Members"
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                Dictionary<int, List<string>> DataListP3 = _dBContext.GroupMemberModels.ToList().ToDictionary(x => x.Id, x => (string.Format("{0}|{1}",
                                                                                      _commonClass.GetGroupDesc(x.GroupId), _commonClass.GetUserTypeIDsDesc(x.UserTypes).Replace("|", "\n"))
                                                                                     .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList()));

                byte[] data = pDFReport.GeneratePDF(
                                          DataList,
                                          "GroupListReport", userName, userId,
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
                                          10,
                                          widthListP3,
                                          ColumnCustomHeightP3
                                          );
                string base64String = Convert.ToBase64String(data);
                string fileName = string.Format("GroupListReport{0}.pdf", DateTime.Now.ToString("MMddyyyy"));

                var printReportResult = new ReturnDownloadPDF
                {
                    PdfDataBase64 = base64String,
                    FileName = fileName
                };
                obj.Data = printReportResult;
                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";
            }
            catch(Exception ex)
            {
                obj.StatusMessage = ex.Message;
            }

            return obj;
        }

        public async Task<ReturnGenericData<ReturnDownloadPDF>> PrintMatrix()
        {
            var obj = new ReturnGenericData<ReturnDownloadPDF>();
            obj.StatusCode = "01";
            try
            {
                var chekReportDetail = _dBContext.MenuModels.Where(e => e.RootMenu == true && e.Isdeleted == false).OrderBy(e => e.MenuDesc);

                //var groupAccessModels = _dBContext.GroupAccessModels.Where(e => e.Isdeleted == false).OrderBy(e => e.GroupDesc);
                var groupAccessModels = _dBContext.GroupAccessModels
                                        .Where(g => !g.Isdeleted)
                                        .AsEnumerable()
                                        .OrderBy(g => g.GroupDesc);


                List<MenuModel> chekReportDetailList = chekReportDetail.ToList();

                string filename = string.Format("attachment; filename=\"MatrixListReport{0}.pdf\"", DateTime.Now.GetDateValue()).Replace("/", "").Replace(":", "").Replace(" ", "");

                var userClaims = _userClaims.GetClaims();
                string userName = userClaims.DisplayName,
                       userId = userClaims.UserID;

                var contentDisposition = filename;// "attachment; filename=\"UserReport.pdf\"";
                string sText = "";
                int width = 500,
                    positionX = 0,
                    positionY = 0,
                    height = 40,
                    itenNo = 0,
                    lineNo = 0,
                    lineTotal = chekReportDetailList.Count;

                List<int> positionXList = new List<int> { };
                List<int> widthList = new List<int> { 300 };
                List<string> sTextList = new List<string> { };
                List<string> sTextListHdr = new List<string> { };

                PdfDocument pdfDocument = new PdfDocument();
                PdfSecuritySettings pdfSecurity;
                pdfDocument.Info.Title = "Matrix List Report";
                pdfDocument.Info.Subject = "Matrix List Report";
                pdfDocument.Info.Author = "Author";
                pdfDocument.Info.Creator = "Creator";
                pdfDocument.Info.CreationDate = DateTime.Now;
                pdfSecurity = pdfDocument.SecuritySettings;
                pdfSecurity.OwnerPassword = "B@nkc0m.c0m.ph";
                Protected(ref pdfSecurity, true);
                Printable(ref pdfSecurity, true);

                PdfPage pdfPage = CreatePage(ref pdfDocument, PageType.LegalLandscapePage);
                XRect xRect = new XRect();
                if (GlobalFontSettings.FontResolver == null)
                    GlobalFontSettings.FontResolver = new CustomFontResolver(new Dictionary<string, string>
                    {
                        { "Arial", @"Fonts/arial.ttf" },
                        { "Arial Bold", @"Fonts/arialbd.ttf" },
                        { "Courier New", @"Fonts/cour.ttf" },
                        { "Courier New Bold", @"Fonts/courbd.ttf" },
                    });

                XGraphics xGraphics = XGraphics.FromPdfPage(pdfPage);
                XTextFormatter xTextFormatter = BeginWrite(ref xGraphics, ref xRect, Alignment.Left);

                #region Matrix Model
                sText = "Matrix List Report";
                positionY = 20;
                positionX = 40;
                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                WriteReportTitle(ref xTextFormatter, ref xRect, sText);
                positionY += height;

                //sTextList = "Menu Desc|_"
                //                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();

                string grpaccess = "", subaccess = "";
                foreach (GroupAccessModel grp in groupAccessModels)
                {
                    grpaccess += grp.GroupDesc + "\n" + "|";
                    widthList.Add(100);
                }
                widthList.Add(widthList[0]);

                sTextListHdr = ("Menu Desc|" + grpaccess)
                                .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();

                height = 40;
                width = 0;
                positionX = 40;
                Alignment alignment = Alignment.Left;
                FontType fontType = FontType.xFontRegular12;
                //foreach (string sTextItem in sTextList)
                foreach (string sTextItem in sTextListHdr)
                {
                    sText = sTextItem;
                    width = widthList[itenNo];
                    MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                    //pDFReport.WriteColumn(sText);
                    Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontBold12, alignment);
                    itenNo++;
                    if (itenNo > 0)
                        alignment = Alignment.Center;
                    //positionXList.Add(positionX);
                    positionX += width;
                }
                positionY += height;

                foreach (MenuModel item in chekReportDetailList)
                {
                    if (lineNo >= 20)
                    {
                        lineNo = 0;
                        pdfPage = CreatePage(ref pdfDocument, PageType.LegalLandscapePage);
                        xGraphics = XGraphics.FromPdfPage(pdfPage);
                        xTextFormatter = BeginWrite(ref xGraphics, ref xRect, Alignment.Left);
                        height = 40;
                        width = 0;
                        positionX = 40;
                        positionY = 20;
                        alignment = Alignment.Left;
                        //sTextList = "Menu Desc|_"
                        //        .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
                        //foreach (string sTextItem in sTextList)
                        foreach (string sTextItem in sTextListHdr)
                        {
                            sText = sTextItem;
                            width = widthList[itenNo];
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            //pDFReport.WriteColumn(sText);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontBold12, alignment);
                            itenNo++;
                            if (itenNo > 0)
                                alignment = Alignment.Center;
                            //positionXList.Add(positionX);
                            positionX += width;
                        }
                        positionY += height;
                    }

                    grpaccess = ""; subaccess = "";
                    foreach (GroupAccessModel grp in groupAccessModels)
                    {
                        grpaccess += grp.MenuIds.Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.GetValue() == item.MenuCode).Contains(true) ? "O|" : "X|";
                    }

                    int menuIDsDescCount = _commonClass.GetMenuIDsDesc(item.SubMenusDesc ?? "").Split("|", StringSplitOptions.RemoveEmptyEntries).Length;
                    
                    for (int col = 0; col < grpaccess.Split("|", StringSplitOptions.RemoveEmptyEntries).Length; col++)
                    {
                        for (int row = 0; row < menuIDsDescCount; row++)
                        {
                            subaccess += grpaccess.Split("|", StringSplitOptions.RemoveEmptyEntries)[col] + "\n";
                        }
                        subaccess += "|";
                        widthList.Add(100);
                    }

                    sTextList = string.Format("{0}|{2}|{1}|{3}|",
                        item.MenuDesc, _commonClass.GetMenuIDsDesc(item.SubMenusDesc ?? "").Replace("|", "\n"), grpaccess, subaccess)
                        .Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();

                    alignment = Alignment.Left;
                    fontType = FontType.xFontBold12;
                    height = 40; width = 0; positionX = 40; itenNo = 0;
                    foreach (string sTextItem in sTextList)
                    {
                        sText = sTextItem;
                        width = widthList[itenNo];
                        height = menuIDsDescCount > 1 ? 15 * menuIDsDescCount : 30;
                        MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                        Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, fontType: fontType, alignment: alignment);
                        itenNo++;
                        if (itenNo == sTextList.Count / 2)
                        {
                            fontType = FontType.xFontRegular12;
                            alignment = Alignment.Left;
                            positionX = 45;
                            positionY += 15;
                        }
                        else
                        {
                            if (positionX == 45)
                                positionX -= 5;


                            fontType = FontType.xFontBold12;
                            positionX += width;
                            alignment = Alignment.Center;
                        }
                    }
                    itenNo = 0;
                    positionY += height;
                    lineNo += menuIDsDescCount > 1 ? menuIDsDescCount : 1;
                }
                #endregion

                EndWrite(pdfDocument);
                byte[] data = StreamByte;
                string base64String = Convert.ToBase64String(data);
                string fileName = string.Format("MatrixListReport{0}.pdf", DateTime.Now.ToString("MMddyyyy"));

                var printReportResult = new ReturnDownloadPDF
                {
                    PdfDataBase64 = base64String,
                    FileName = fileName
                };
                obj.Data = printReportResult;
                obj.StatusCode = "00";
                obj.StatusMessage = "SUCCESS";

            }
            catch (Exception ex)
            {
                obj.StatusMessage = ex.Message;

            }
            return obj;
        }

        private bool GroupAccessModelExists(string sGroupID)
        {
            return _dBContext.GroupAccessModels.Any(e => e.GroupId == sGroupID && e.Isdeleted == false);
        }
        private bool GroupMemberModelExists(string sGroupID)
        {
            return _dBContext.GroupMemberModels.Any(e => e.GroupId == sGroupID && e.Isdeleted == false);
        }
        private bool GroupModelExists(int id)
        {
            return _dBContext.GroupModels.Any(e => e.Id == id && e.Isdeleted == false);
        }

        private bool GroupModelExists(string sGroupCode)
        {
            return _dBContext.GroupModels.Any(e => e.GroupCode == sGroupCode && e.Isdeleted == false);
        }

        private bool GroupMemberActive(string GroupID)
        {
            var lstGrpMember = _dBContext.GroupMemberModels.SingleOrDefault(e => e.GroupId == GroupID && e.Isdeleted == false);
            if (lstGrpMember.IsNull())
            {
                return false;
            }
            else
            {
                bool result = false;
                foreach (string sUserTypeCode in lstGrpMember.UserTypes.Split(',').ToList())
                {
                    result |= _dBContext.UserTypeModels.Any(e => e.UserTypeCode == sUserTypeCode && e.Isdeleted == false);
                }
                return result;
            }
        }

        private void DeleteGroupAccessAndMembers(string sGroupId)
        {
            var gm = _dBContext.GroupAccessModels.FirstOrDefault(e => e.GroupId == sGroupId && e.Isdeleted == false);
            var ga = _dBContext.GroupMemberModels.FirstOrDefault(e => e.GroupId == sGroupId && e.Isdeleted == false);
            if (gm != null)
            {
                gm.Isdeleted = true;
                _dBContext.Update(gm);
            }

            if (ga != null)
            {
                ga.Isdeleted = true;
                _dBContext.Update(ga);
            }
        }

        #region PDF Methods
        public PdfPage CreatePage(ref PdfDocument pdfDocument, PageType pageType = PageType.LegalPortraitPage)
        {
            try
            {
                if (pageType == PageType.LetterLandscapePage)
                    return pdfDocument.AddPage(new PdfPage { Size = PageSize.Letter, Orientation = PageOrientation.Landscape });

                if (pageType == PageType.LetterPortraitPage)
                    return pdfDocument.AddPage(new PdfPage { Size = PageSize.Letter, Orientation = PageOrientation.Portrait });

                if (pageType == PageType.LegalLandscapePage)
                    return pdfDocument.AddPage(new PdfPage { Size = PageSize.Legal, Orientation = PageOrientation.Landscape });

                if (pageType == PageType.LegalPortraitPage)
                    return pdfDocument.AddPage(new PdfPage { Size = PageSize.Legal, Orientation = PageOrientation.Portrait });

                return pdfDocument.AddPage();
            }
            catch
            {
                throw;
            }
        }

        public XTextFormatter BeginWrite(ref XGraphics xGraphics, ref XRect xRect, Alignment alignment = Alignment.Justify, FontType fontType = FontType.xFontRegular12)
        {
            XTextFormatter xTextFormatter = new XTextFormatter(xGraphics)
            {
                Alignment = GetParagraphAlignment(alignment),
                Font = GetFontType(fontType),
                LayoutRectangle = xRect
            };
            return xTextFormatter;
        }

        public void EndWrite(PdfDocument pdfDocument, string path = "")
        {
            if (pdfDocument.PageCount == 0)
                return;

            if (path.GetValue().Length > 0)
            {
                pdfDocument.Save(path);
            }
            else
            {
                using (MemoryStream _Stream = new MemoryStream())
                {
                    pdfDocument.Save(_Stream);
                    //Stream = _Stream;
                    StreamLength = _Stream.Length;
                    StreamByte = _Stream.ToArray();
                    _Stream.Close();
                }
            }

        }

        public void MakeLayoutRect(ref XGraphics xGraphics, ref XRect xRect, double x, double y, double width, double height, bool bBorder = false)
        {
            xRect = new XRect(x, y, width, height);
            if (bBorder)
                xGraphics.DrawRectangle(xPen, xRect);
        }

        public void WriteReportTitle(ref XTextFormatter xTextFormatter, ref XRect xRect, string text, FontType fontType = FontType.xFontBold35)
        {
            xTextFormatter.DrawString(text, GetFontType(fontType), XBrushes.Black, xRect);
        }

        public void WriteColumn(ref XTextFormatter xTextFormatter, ref XRect xRect, string text, FontType fontType = FontType.xFontBold12)
        {
            xTextFormatter.DrawString(text, GetFontType(fontType), XBrushes.Black, xRect);
        }
        public void Write(ref XGraphics xGraphics, ref XTextFormatter xTextFormatter, ref XRect xRect, string text, FontType fontType = FontType.xFontRegular12,
                            Alignment alignment = Alignment.Justify,
                            bool line = false, PenWidth penWidth = PenWidth.xPen,
                            PenPos penPos1 = PenPos.Center, PenPos penPos2 = PenPos.Center)
        {

            if (line)
            {
                double y1 = 0, y2 = 0;
                switch (penPos1)
                {
                    case PenPos.Center: y1 = xRect.Center.Y; break;
                    case PenPos.Top: y1 = xRect.Top; break;
                    case PenPos.Bottom: y1 = xRect.Bottom; break;
                }

                switch (penPos2)
                {
                    case PenPos.Center: y2 = xRect.Center.Y; break;
                    case PenPos.Top: y2 = xRect.Top; break;
                    case PenPos.Bottom: y2 = xRect.Bottom; break;
                }
                xGraphics.DrawLine(GetPenWidth(penWidth), xRect.Left, y1, xRect.Right, y2);
            }

            xTextFormatter.Alignment = GetParagraphAlignment(alignment);
            xTextFormatter.DrawString(text, GetFontType(fontType), XBrushes.Black, xRect);
        }

        public void WriteRed(string text, FontType fontType = FontType.xFontRegular12,
                            Alignment alignment = Alignment.Justify,
                            bool line = false, PenWidth penWidth = PenWidth.xPen,
                            PenPos penPos1 = PenPos.Center, PenPos penPos2 = PenPos.Center)
        {

            if (line)
            {
                double y1 = 0, y2 = 0;
                switch (penPos1)
                {
                    case PenPos.Center: y1 = xRect.Center.Y; break;
                    case PenPos.Top: y1 = xRect.Top; break;
                    case PenPos.Bottom: y1 = xRect.Bottom; break;
                }

                switch (penPos2)
                {
                    case PenPos.Center: y2 = xRect.Center.Y; break;
                    case PenPos.Top: y2 = xRect.Top; break;
                    case PenPos.Bottom: y2 = xRect.Bottom; break;
                }
                xGraphics.DrawLine(GetPenWidth(penWidth), xRect.Left, y1, xRect.Right, y2);
            }

            xTextFormatter.Alignment = GetParagraphAlignment(alignment);
            xTextFormatter.DrawString(text, GetFontType(fontType), XBrushes.Red, xRect);
        }

        public void WriteBlue(string text, FontType fontType = FontType.xFontRegular12,
                            Alignment alignment = Alignment.Justify,
                            bool line = false, PenWidth penWidth = PenWidth.xPen,
                            PenPos penPos1 = PenPos.Center, PenPos penPos2 = PenPos.Center)
        {

            if (line)
            {
                double y1 = 0, y2 = 0;
                switch (penPos1)
                {
                    case PenPos.Center: y1 = xRect.Center.Y; break;
                    case PenPos.Top: y1 = xRect.Top; break;
                    case PenPos.Bottom: y1 = xRect.Bottom; break;
                }

                switch (penPos2)
                {
                    case PenPos.Center: y2 = xRect.Center.Y; break;
                    case PenPos.Top: y2 = xRect.Top; break;
                    case PenPos.Bottom: y2 = xRect.Bottom; break;
                }
                xGraphics.DrawLine(GetPenWidth(penWidth), xRect.Left, y1, xRect.Right, y2);
            }

            xTextFormatter.Alignment = GetParagraphAlignment(alignment);
            xTextFormatter.DrawString(text, GetFontType(fontType), XBrushes.Blue, xRect);
        }

        public void WriteImage(Stream stream)
        {
            XImage image = XImage.FromStream(stream);
            xGraphics.DrawImage(image, xRect);
        }

        public void WriteImage(string filePath)
        {
            XImage image = XImage.FromFile(filePath);
            xGraphics.DrawImage(image, xRect);
        }

        public void Protected(ref PdfSecuritySettings pdfSecurity, bool value)
        {
            //pdfSecurity.PermitAccessibilityExtractContent = !value;
            pdfSecurity.PermitAnnotations = !value;
            pdfSecurity.PermitAssembleDocument = !value;
            pdfSecurity.PermitExtractContent = !value;
            pdfSecurity.PermitFormsFill = !value;
            pdfSecurity.PermitFullQualityPrint = !value;
            pdfSecurity.PermitModifyDocument = !value;
            pdfSecurity.PermitPrint = !value;
        }
        public void Printable(ref PdfSecuritySettings pdfSecurity, bool value)
        {
            pdfSecurity.PermitPrint = value;
        }
        public class CustomFontResolver : IFontResolver
        {
            private readonly Dictionary<string, string> _fontFiles;

            public CustomFontResolver(Dictionary<string, string> fontFiles)
            {
                _fontFiles = fontFiles;
            }

            public byte[] GetFont(string faceName)
            {
                if (_fontFiles.ContainsKey(faceName))
                {
                    string fontPath = _fontFiles[faceName];
                    return System.IO.File.ReadAllBytes(fontPath);
                }
                return null;
            }

            public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {
                if (_fontFiles.ContainsKey(familyName))
                {
                    string fontPath = _fontFiles[familyName];
                    return new FontResolverInfo(familyName, false, false);
                }
                return null;
            }
        }


        #endregion

    }
}
