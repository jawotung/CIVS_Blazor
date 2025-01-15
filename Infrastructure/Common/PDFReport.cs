using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf.Security;
using PdfSharp.Pdf;
using PdfSharp;
using System.Reflection.Metadata;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using PdfSharp.Fonts;
using System.Resources;
using System.IO;
using PdfSharp.Pdf.Advanced;
using System.Drawing;
using System.Reflection.PortableExecutable;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Application.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PdfSharp.Snippets.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure;

namespace Infrastructure.Common
{
    public class PDFReport
    {
        #region Variable
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
        private static readonly XFont xFontArialBold6 = new XFont("Arial Bold", 6, XFontStyleEx.Bold);
        private static readonly XFont xFontArialBold8 = new XFont("Arial Bold", 8, XFontStyleEx.Bold);
        private static XPen xPen = new XPen(XColors.Black, 1);
        private static XPen xPen2 = new XPen(XColors.Black, 2);
        private static XPen xPen3 = new XPen(XColors.Black, 3);
        private static XPen xPen4 = new XPen(XColors.Black, 4);
        public enum PageType
        {
            LetterLandscapePage,
            LetterPortraitPage,
            LegalLandscapePage,
            LegalPortraitPage
        }
        public enum Alignment
        {
            Left,
            Right,
            Center,
            Justify
        }
        public enum FontType
        {
            xFontRegular6,
            xFontRegular8,
            xFontRegular10,
            xFontRegular12,
            xFontBold6,
            xFontBold8,
            xFontBold10,
            xFontBold12,
            xFontBold25,
            xFontBold35,

            xFontArialBold6,
            xFontArialBold8
        }
        public enum PenPos
        {
            Top,
            Center,
            Bottom
        }
        public enum PenWidth
        {
            xPen,
            xPen2,
            xPen3,
            xPen4
        }

        public long StreamLength;
        public byte[] StreamByte;

        #endregion
        public byte[] GeneratePDF(Dictionary<int, List<string>> DataList,
                                string Title, string Author, string Creator,
                                string TitleHeader,
                                List<string> Header,
                                List<int> widthList,
                                int ColumnCustomHeight = 0,
                                int lineNoLimit = 10,
                                string TitleHeaderP2 = "",
                                List<string>? HeaderP2 = null,
                                Dictionary<int, List<string>>? DataListP2 = null,
                                int lineNoLimitP2 = 10,
                                List<int>? widthListP2 = null,
                                int ColumnCustomHeightP2 = 0,
                                string TitleHeaderP3 = "",
                                List<string>? HeaderP3 = null,
                                Dictionary<int, List<string>>? DataListP3 = null,
                                int lineNoLimitP3 = 10,
                                List<int>? widthListP3 = null,
                                int ColumnCustomHeightP3 = 0,
                                bool setProtection = false,
                                PageType PageDesign = PDFReport.PageType.LegalLandscapePage,
                                Alignment alignment = Alignment.Justify, FontType fontType = FontType.xFontRegular12,
                                bool bBorder = false)
        {
            byte[] StreamByte;
            try
            {
                int width = 500,
                   positionX = 40,
                   positionY = 20,
                   height = 40,
                   itenNo = 0,
                   lineNo = 0;
                int ActionCount = 0;

                List<int> positionXList = new List<int> { };
                XRect xRect = new XRect();
                if(GlobalFontSettings.FontResolver == null)
                    GlobalFontSettings.FontResolver = new CustomFontResolver(new Dictionary<string, string>
                    {
                        { "Arial", @"Fonts/arial.ttf" },
                        { "Arial Bold", @"Fonts/arialbd.ttf" },
                        { "Courier New", @"Fonts/cour.ttf" },
                        { "Courier New Bold", @"Fonts/courbd.ttf" },
                    });
                
                PdfDocument pdfDocument = new PdfDocument();
                pdfDocument.Info.Title = Title;
                pdfDocument.Info.Subject = Title;
                pdfDocument.Info.Author = Author;
                pdfDocument.Info.Creator = Creator;
                pdfDocument.Info.CreationDate = DateTime.Now;
                WriteBody(ref pdfDocument, ref xRect,
                                DataList,
                                Title, Author, Creator,
                                TitleHeader,
                                Header,
                                widthList,
                                ColumnCustomHeight,
                                ref lineNoLimit,
                                ref setProtection,
                                ref PageDesign,
                                ref alignment, ref fontType,
                                ref bBorder,
                                ref positionX, ref positionY, ref width, ref height, ref itenNo,
                                ref positionXList, ref lineNo, ref ActionCount
                );
                #region 2nd List
                if(!string.IsNullOrEmpty(TitleHeaderP2))
                {

                    width = 500;
                    height = 40;
                    positionY = 20;
                    positionX = 40;
                    itenNo = 0;
                    lineNo = 0;
                    positionXList.Clear();
                    widthList = widthListP2;
                    TitleHeader = TitleHeaderP2;
                    Header = HeaderP2;
                    DataList = DataListP2;
                    ColumnCustomHeight = ColumnCustomHeightP2;
                    WriteBody(ref pdfDocument, ref xRect,
                                    DataList,
                                    Title, Author, Creator,
                                    TitleHeader,
                                    Header,
                                    widthList,
                                    ColumnCustomHeight,
                                    ref lineNoLimit,
                                    ref setProtection,
                                    ref PageDesign,
                                    ref alignment, ref fontType,
                                    ref bBorder,
                                    ref positionX, ref positionY, ref width, ref height, ref itenNo,
                                    ref positionXList, ref lineNo, ref ActionCount
                    );
                }
                #endregion
                #region 3rd List
                if (!string.IsNullOrEmpty(TitleHeaderP3))
                {
                    width = 500;
                    height = 40;
                    positionY = 20;
                    positionX = 40;
                    itenNo = 0;
                    lineNo = 0;
                    positionXList.Clear();
                    widthList = widthListP3;
                    TitleHeader = TitleHeaderP3;
                    Header = HeaderP3;
                    DataList = DataListP3;
                    ColumnCustomHeight = ColumnCustomHeightP3;
                    WriteBody(ref pdfDocument, ref xRect,
                                    DataList,
                                    Title, Author, Creator,
                                    TitleHeader,
                                    Header,
                                    widthList,
                                    ColumnCustomHeight,
                                    ref lineNoLimit,
                                    ref setProtection,
                                    ref PageDesign,
                                    ref alignment, ref fontType,
                                    ref bBorder,
                                    ref positionX, ref positionY, ref width, ref height, ref itenNo,
                                    ref positionXList, ref lineNo, ref ActionCount
                    );
                }
                #endregion
                StreamByte = EndWrite(pdfDocument);
                return StreamByte;
            }
            catch
            {
                throw;
            }
        }
        public void WriteBody(ref PdfDocument pdfDocument,ref XRect xRect,
                                Dictionary<int, List<string>> DataList,
                                string Title, string Author, string Creator,
                                string TitleHeader,
                                List<string> Header,
                                List<int> widthList,
                                int ColumnCustomHeight,
                                ref int lineNoLimit,
                                ref bool setProtection,
                                ref PageType PageDesign,
                                ref Alignment alignment, ref FontType fontType,
                                ref bool bBorder,
                                ref int positionX, ref int positionY, ref int width, ref int height, ref int itenNo,
                                ref List<int> positionXList, ref int lineNo,ref int ActionCount
            )
        {
            try
            {

                PdfPage pdfPage = CreatePage(ref pdfDocument, PageDesign);
                XGraphics xGraphics = XGraphics.FromPdfPage(pdfPage);
                XTextFormatter xTextFormatter = BeginWrite(ref xGraphics, ref xRect, alignment, fontType);
                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                WriteReportTitle(ref xTextFormatter, ref xRect, TitleHeader);

                positionY += height;
                height = 40;
                width = 0;
                positionX = 40;
                PopulateTableHeader(Header,
                                    ref xGraphics, ref xRect, ref xTextFormatter,
                                    ref positionX, ref positionY, ref width, ref height, ref itenNo,
                                    ref widthList, ref positionXList);
                positionY += height;
                foreach (var data in DataList)
                {
                    if (lineNo >= lineNoLimit)
                    {
                        lineNo = 0;
                        pdfPage = CreatePage(ref pdfDocument, PageDesign);
                        xGraphics = XGraphics.FromPdfPage(pdfPage);
                        xTextFormatter = BeginWrite(ref xGraphics, ref xRect, alignment, fontType);
                        height = 40;
                        width = 0;
                        positionX = 40;
                        positionY = 20;
                        PopulateTableHeader(Header,
                                            ref xGraphics, ref xRect, ref xTextFormatter,
                                            ref positionX, ref positionY, ref width, ref height, ref itenNo,
                                            ref widthList, ref positionXList);
                        positionY += height;
                    }
                    if (ColumnCustomHeight != 0)
                    {
                        ActionCount = data.Value[ColumnCustomHeight].Split("\n", StringSplitOptions.RemoveEmptyEntries).Length;
                        height = ActionCount > 1 ? 20 * ActionCount : 40;
                    }
                    height = 40; width = 0; positionX = 40; itenNo = 0;
                    foreach (string x in data.Value)
                    {
                        width = width = widthList[itenNo];
                        if (ColumnCustomHeight != 0)
                        {
                            height = ActionCount > 1 ? 20 * ActionCount : 40;
                        }
                        MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                        Write(ref xGraphics, ref xTextFormatter, ref xRect, x);
                        itenNo++;
                        if (itenNo < data.Value.Count)
                            positionX = positionXList[itenNo];
                    }
                    itenNo = 0;
                    positionY += height;
                    if (Title == "MenuListReport" || TitleHeader == "Menu Access")
                        lineNo += ActionCount > 1 ? ActionCount : 1;
                    else
                        lineNo++;
                }
            }
            catch
            {
                throw;
            }
        }
        public PdfPage CreatePage(ref PdfDocument pdfDocument,PageType pageType = PageType.LegalPortraitPage)
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
        public void Write(ref XGraphics xGraphics, ref XTextFormatter xTextFormatter, ref XRect xRect, 
                          string text, FontType fontType = FontType.xFontRegular12,
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
        public void WriteRed(ref XGraphics xGraphics, ref XTextFormatter xTextFormatter, ref XRect xRect,
                          string text, FontType fontType = FontType.xFontRegular12,
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

        public void WriteImage(ref XGraphics xGraphics, ref XRect xRect, byte[] imageBytes)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
                {
                    using (MemoryStream tempStream = new MemoryStream())
                    {
                        image.Save(tempStream, System.Drawing.Imaging.ImageFormat.Jpeg); // Change ImageFormat as needed
                        tempStream.Position = 0;
                        XImage pdfImage = XImage.FromStream(tempStream);
                        xGraphics.DrawImage(pdfImage, xRect); // Adjust position and size as needed
                    }
                }
            }
        }
        public void WriteImage(ref XGraphics xGraphics, ref XRect xRect, string filePath)
        {

            XImage image = XImage.FromFile(filePath);
            xGraphics.DrawImage(image, xRect);
        }
        public void PopulateTableHeader(List<string> Header, 
                                        ref XGraphics xGraphics, ref XRect xRect, ref XTextFormatter xTextFormatter,
                                        ref int positionX, ref int positionY, ref int width, ref int height, ref int itenNo,
                                        ref List<int> widthList, ref List<int> positionXList)
        {

            foreach (string x in Header)
            {
                width = widthList[itenNo];
                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                WriteColumn(ref xTextFormatter, ref xRect, x);
                itenNo++;
                positionXList.Add(positionX);
                positionX += width;
            }
        }
        public byte[] EndWrite(PdfDocument pdfDocument,string path = "")
        {
            byte[] StreamByte;
            try
            {
                using (MemoryStream _Stream = new MemoryStream())
                {
                    pdfDocument.Save(_Stream);
                    StreamLength = _Stream.Length;
                    StreamByte = _Stream.ToArray();
                    StreamByte = _Stream.ToArray();
                    _Stream.Close();
                }
            }
            catch
            {
                throw;
            }
            return StreamByte;
        }
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

        #region Report Image

        public byte[] GeneratePDFWithImage(
                                string key, string? dateFromFilter, string? dateToFilter, string? BSRTNFilter,
                                string ImageFolder,
                                List<InwardClearingReportModel> DataList,
                                string Title, string Author, string Creator,
                                PageType PageDesign = PDFReport.PageType.LegalLandscapePage,
                                Alignment alignment = Alignment.Justify, FontType fontType = FontType.xFontRegular12)
        {
            byte[] StreamByte;
            try
            {
                DateTime dateFrom = dateFromFilter.ToDate().IsValidDate() ? dateFromFilter.ToDate() : DateTime.Now.Date,
                       dateTo = dateToFilter.ToDate().IsValidDate() ? dateToFilter.ToDate() : DateTime.Now.Date;

                string reportFilenNme = key.IsNull() ? "" :
                       key.StartsWith("LargeAmount") ? "LargeAmount" : key.StartsWith("WithTechnicalities") ? "WithTechnicalities" : key.StartsWith("MCImages") ? "MCImages" : "";
                string filename = string.Format("{0}Report{1}.pdf", reportFilenNme, DateTime.Now.GetDateValue()).Replace("/", "").Replace(":", "").Replace(" ", "");
                var contentDisposition = string.Format("attachment; filename=\"{0}\"", filename);
                string sText = "";
                string filePath = "";
                string imageTemp = "'";
                string saveFilePath = ImageFolder;
                saveFilePath = Path.Combine(saveFilePath, filename);
                string sTmpBranch = "",
                        sTotalItems = "0",
                        sTotalAmount = "0",
                        sTmpAmt = "0",
                        userId = Author;
                string sTmpBranchName = "",
                       sRepTitle1 = "BANK OF COMMERCE - Central Clearing",
                       sRepTitle2 = key.StartsWith("LargeAmount")
                           ? string.Format("LARGE AMOUNT ITEMS (w/Image) for Transaction Date: {0} - {1}", dateFrom.ToShortDateString(), dateTo.ToShortDateString())
                           : (key == "WithTechnicalities")
                               ? "ITEM LIST with TECHNICALITIES (w/Image)"
                               : (key == "MCImages")
                                   ? "MC Image with Details"
                                   : "",
               sRepTitle3 = key.StartsWith("LargeAmount")
                ? ""
                : (key == "WithTechnicalities")
                               ? string.Format("Transaction Date: {0} - {1}", dateFrom.ToShortDateString(), dateTo.ToShortDateString())
                : (key == "MCImages")
                                   ? string.Format("Transaction Date: {0} - {1}", dateFrom.ToShortDateString(), dateTo.ToShortDateString())
                                   : "";
                int width = 0,
                        positionX = 0,
                        positionY = 0,
                        height = 40,
                        lineNo = 0;

                List<int> positionXList = new List<int> { };
                XRect xRect = new XRect();
                if (GlobalFontSettings.FontResolver == null)
                    GlobalFontSettings.FontResolver = new CustomFontResolver(new Dictionary<string, string>
                    {
                        { "Arial", @"Fonts/arial.ttf" },
                        { "Arial Bold", @"Fonts/arialbd.ttf" },
                        { "Courier New", @"Fonts/cour.ttf" },
                        { "Courier New Bold", @"Fonts/courbd.ttf" },
                    });

                PdfDocument pdfDocument = new PdfDocument();
                pdfDocument.Info.Title = Title;
                pdfDocument.Info.Subject = Title;
                pdfDocument.Info.Author = Author;
                pdfDocument.Info.Creator = Creator;
                pdfDocument.Info.CreationDate = DateTime.Now;

                Alignment xAlignment = key.StartsWith("LargeAmount") ? Alignment.Right : Alignment.Left;

                sTmpBranch = "";
                int totalCheques = 0, pageNo = 1, totalPages = 1;

                if (key == "MCImages")
                {
                    PdfPage pdfPage = CreatePage(ref pdfDocument, PageDesign);
                    XGraphics xGraphics = XGraphics.FromPdfPage(pdfPage);
                    XTextFormatter xTextFormatter = BeginWrite(ref xGraphics, ref xRect, alignment, fontType);

                    #region create pdf                
                    sText = "";
                    positionX = 20;

                    if (DataList.Count != 0)
                    {
                        foreach (InwardClearingReportModel item in DataList)
                        {
                            filePath = ImageFolder;
                            if (lineNo == 0 && sTmpBranch == "")
                            {
                                sTmpBranch = item.BranchBRSTN;
                                sTmpBranchName = item.BranchName;

                                totalCheques = item.TotalItems.ToInt();
                                totalPages = (totalCheques <= 4) ? 1 : (totalCheques / 4) + ((totalCheques % 4) > 0 ? 1 : 0);
                                PopulateReportImageHeaderMCImages(sRepTitle1, sRepTitle2, sRepTitle3,
                                                          totalCheques, pageNo, totalPages,
                                                          sTmpBranch, sTmpBranchName,
                                        ref xGraphics, ref xRect, ref xTextFormatter,
                                        ref positionX, ref positionY, ref width, ref height);
                                sText = "";

                            }
                            if (lineNo == 3 || sTmpBranch != item.BranchBRSTN)
                            {
                                pageNo++;
                                #region footer
                                if (sTmpBranch != item.BranchBRSTN)
                                {
                                    PopulateReportImageFooterMCImages(sTmpBranch, sTmpBranchName, userId, item.TotalItems,
                                       ref pageNo, ref totalCheques, ref totalPages,
                                       ref xGraphics, ref xRect, ref xTextFormatter,
                                       ref positionX, ref positionY, ref width, ref height);
                                }
                                sTmpBranch = item.BranchBRSTN;
                                sTmpBranchName = item.BranchName;
                                #endregion
                                lineNo = 0;
                                pdfPage = CreatePage(ref pdfDocument, PageType.LegalLandscapePage);
                                xGraphics = XGraphics.FromPdfPage(pdfPage);
                                xTextFormatter = BeginWrite(ref xGraphics, ref xRect, Alignment.Left, fontType);
                                PopulateReportImageHeaderMCImages(sRepTitle1, sRepTitle2, sRepTitle3,
                                                          totalCheques, pageNo, totalPages,
                                                          sTmpBranch, sTmpBranchName,
                                        ref xGraphics, ref xRect, ref xTextFormatter,
                                        ref positionX, ref positionY, ref width, ref height);
                            }
                            height = 130; width = 350; positionX = 220;
                            if (item.FrontImage.GetValue() != "")
                            {
                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                imageTemp = item.FrontImage.GetValue();

                                if (imageTemp.IsBase64String())
                                    WriteImage(ref xGraphics, ref xRect, Convert.FromBase64String(imageTemp));
                                else
                                {
                                    filePath = ImageFolder;
                                    filePath = Path.Combine(filePath, imageTemp);
                                    WriteImage(ref xGraphics, ref xRect, filePath);
                                }
                            }
                            positionX += width + 20;
                            if (item.BackImage.GetValue() != "")
                            {
                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                imageTemp = item.BackImage.GetValue();

                                if (imageTemp.IsBase64String())
                                    WriteImage(ref xGraphics, ref xRect, Convert.FromBase64String(imageTemp));
                                else
                                {
                                    filePath = ImageFolder;
                                    filePath = Path.Combine(filePath, imageTemp);
                                    WriteImage(ref xGraphics, ref xRect, filePath);
                                }
                            }

                            positionX = 20;
                            positionY += 16;
                            width = 70;
                            height = 40;

                            sText = "Bus Date:";
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

                            positionY += 20;
                            sText = "BRSTN:";
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

                            positionY += 20;
                            sText = "Account No.:";
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

                            positionY += 20;
                            sText = "Amount:";
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

                            positionY -= 60;
                            positionX += width + 2;

                            sText = item.EffectivityDate;
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

                            positionY += 20;
                            sText = item.BranchBRSTN;
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

                            positionY += 20;
                            sText = item.AccountNumber;
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

                            positionY += 20;
                            sText = item.CheckAmount.ToString("#,##0.00");
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);


                            positionY += 80;
                            lineNo++;
                        }
                        #region Footer
                        PopulateReportImageFooterMCImages(sTmpBranch, sTmpBranchName, userId, "0",
                           ref pageNo, ref totalCheques, ref totalPages,
                           ref xGraphics, ref xRect, ref xTextFormatter,
                           ref positionX, ref positionY, ref width, ref height);
                        #endregion
                    }
                    else
                    {
                        #region reptitle
                        PopulateReportImageHeader(sRepTitle1, sRepTitle2, sRepTitle3,
                                                  totalCheques, pageNo, totalPages,
                                                  sTmpBranch, sTmpBranchName,
                                ref xGraphics, ref xRect, ref xTextFormatter,
                                ref positionX, ref positionY, ref width, ref height);
                        sText = "";
                        #endregion
                        #region Footer
                        PopulateReportImageFooter(sTmpBranch, sTmpBranchName, userId, "0",
                           ref pageNo, ref totalCheques, ref totalPages,
                           ref xGraphics, ref xRect, ref xTextFormatter,
                           ref positionX, ref positionY, ref width, ref height);
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    PdfPage pdfPage = CreatePage(ref pdfDocument, PageType.LetterPortraitPage);
                    XGraphics xGraphics = XGraphics.FromPdfPage(pdfPage);
                    XTextFormatter xTextFormatter = BeginWrite(ref xGraphics, ref xRect, Alignment.Left);
                    #region create pdf                
                    sText = "";
                    positionX = 20;

                    if (DataList.Count != 0)
                    {
                        foreach (InwardClearingReportModel item in DataList)
                        {
                            if (lineNo == 0 && sTmpBranch == "")
                            {
                                sTmpBranch = item.BranchBRSTN;
                                sTmpBranchName = item.BranchName;

                                totalCheques = item.TotalItems.ToInt();
                                totalPages = (totalCheques <= 4) ? 1 : (totalCheques / 4) + ((totalCheques % 4) > 0 ? 1 : 0);
                                PopulateReportImageHeader(sRepTitle1, sRepTitle2, sRepTitle3,
                                                          totalCheques, pageNo, totalPages,
                                                          sTmpBranch, sTmpBranchName,
                                                          ref xGraphics, ref xRect, ref xTextFormatter,
                                                          ref positionX, ref positionY, ref width, ref height);


                            }

                            if (lineNo == 4 || sTmpBranch != item.BranchBRSTN)
                            {
                                pageNo++;
                                #region footer
                                if (sTmpBranch != item.BranchBRSTN)
                                {
                                    PopulateReportImageFooter(sTmpBranch, sTmpBranchName, userId, item.TotalItems,
                                       ref pageNo, ref totalCheques, ref totalPages,
                                       ref xGraphics, ref xRect, ref xTextFormatter,
                                       ref positionX, ref positionY, ref width, ref height);
                                }
                                sTmpBranch = item.BranchBRSTN;
                                sTmpBranchName = item.BranchName;
                                #endregion
                                lineNo = 0;

                                pdfPage = CreatePage(ref pdfDocument, PageType.LetterPortraitPage);
                                xGraphics = XGraphics.FromPdfPage(pdfPage);
                                xTextFormatter = BeginWrite(ref xGraphics, ref xRect, Alignment.Left);

                                PopulateReportImageHeader(sRepTitle1, sRepTitle2, sRepTitle3,
                                                          totalCheques, pageNo, totalPages,
                                                          sTmpBranch, sTmpBranchName,
                                                          ref xGraphics, ref xRect, ref xTextFormatter,
                                                          ref positionX, ref positionY, ref width, ref height);
                            }

                            height = 130; width = 350; positionX = 20;

                            if (item.FrontImage.GetValue() != "")
                            {
                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                imageTemp = item.FrontImage.GetValue();

                                if (imageTemp.IsBase64String())
                                    WriteImage(ref xGraphics, ref xRect, Convert.FromBase64String(imageTemp));
                                else
                                {
                                    filePath = ImageFolder;
                                    filePath = Path.Combine(filePath, imageTemp);
                                    WriteImage(ref xGraphics, ref xRect, filePath);
                                }
                            }

                            positionX = width + 10;
                            positionY += 16;
                            width = 70;
                            height = 40;

                            sText = "Account No.:";
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Right);

                            positionY += 20;
                            sText = "Check No.:";
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Right);

                            positionY += 20;
                            sText = "TC:";
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Right);

                            positionY += 20;
                            sText = "Amount:";
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Right);

                            positionY += 20;
                            sText = key.StartsWith("LargeAmount") ? "C. Clrg. Status" : (item.ReAssignReason != "") ? "ReAssigned By:" : "Verified By:";//"Rejected By:";
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Right);

                            if (key == "WithTechnicalities")
                            {
                                positionY += 20;
                                sText = "Timestamp:";
                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Right);
                            }

                            positionY -= (key == "WithTechnicalities") ? 100 : 80;
                            positionX += width + 2;

                            sText = item.AccountNumber;
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontRegular8, xAlignment);

                            positionY += 20;
                            sText = item.CheckNumber;
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontRegular8, xAlignment);

                            positionY += 20;
                            sText = "000";
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontRegular8, xAlignment);

                            positionY += 20;
                            sText = item.CheckAmount.ToString("#,##0.00");
                            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontRegular8, xAlignment);


                            if (key == "WithTechnicalities")
                            {
                                positionY += 20;
                                sText = item.VerifiedBy;
                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontRegular8, xAlignment);

                                positionY += 20;
                                width = 140;
                                sText = item.VerifiedDateTime;
                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontRegular8, xAlignment);

                                positionY += 20;
                                width = 560;
                                positionX = 20;
                                height = 12;
                                sText = "Technicalities";
                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

                                sText = "Branch Status";
                                Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Center);

                                positionX = 20;
                                width = 280;
                                positionY += 12;
                                sText = item.ReAssignReason;// string.Format("{0} {1}", item.RejectReason, );
                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                WriteRed(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold6, Alignment.Left, true, PenWidth.xPen, PenPos.Bottom, PenPos.Bottom);

                                positionX = 250;
                                width = 350;
                                sText = item.ChequeStats[ChequeStats.BrAccept] != "" ? item.ChequeStats[ChequeStats.BrAccept] :
                                            item.ChequeStats[ChequeStats.BrReject] != "" ? item.ChequeStats[ChequeStats.BrReject] : "";
                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                WriteRed(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold6, Alignment.Left);
                            }
                            else
                            {
                                positionY += 20;

                                sText = item.ChequeStats[ChequeStats.Accept] != "" ? "Accepted" : item.ChequeStats[ChequeStats.COAccept] != "" ? "Accepted" :
                                   item.ChequeStats[ChequeStats.Reject] != "" ? "Rejected" : item.ChequeStats[ChequeStats.COReject] != "" ? "Rejected" :
                                     item.ChequeStats[ChequeStats.ReAssign] != "" ? "ReAssigned" : item.ChequeStats[ChequeStats.COReAssign] != "" ? "ReAssigned" :
                                        item.ChequeStats[ChequeStats.ReferToOfficer] != "" ? "Next Level Approver" : "Open";
                                width = 100;

                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontRegular8, xAlignment);

                                positionY += 10;
                                positionX -= 70;
                                width = 140;

                                sText = item.ChequeStats[ChequeStats.Reject] != "" ? item.ChequeStats[ChequeStats.Reject] : item.ChequeStats[ChequeStats.COReject] != "" ? item.ChequeStats[ChequeStats.COReject] :
                                            item.ChequeStats[ChequeStats.ReAssign] != "" ? item.ChequeStats[ChequeStats.ReAssign] : item.ChequeStats[ChequeStats.COReAssign] != "" ? item.ChequeStats[ChequeStats.COReAssign] :
                                                item.ChequeStats[ChequeStats.ReferToOfficer] != "" ? item.ChequeStats[ChequeStats.ReferToOfficer] : "";
                                //sText = string.Format("({0} By {1} On {2})", item.ReAssignReason, item.VerifiedBy, item.VerifiedDateTime);
                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontRegular6, xAlignment);

                                positionY += 30;
                                width = 560;
                                positionX = 20;
                                height = 12;

                                //sText = (item.ReAssignReason == "") ? "Branch Status" :
                                //    string.Format("Branch Status {0}",
                                //            (item.CheckStatus == "Reject") ? string.Format("Rejected - {0}", item.RejectReason)
                                //                                           : (item.CheckStatus == "Accept") ? "Accepted" : ""
                                //    );
                                sText = item.ChequeStats[ChequeStats.BrAccept] != "" ? item.ChequeStats[ChequeStats.BrAccept] :
                                            item.ChequeStats[ChequeStats.BrReject] != "" ? item.ChequeStats[ChequeStats.BrReject] : "";
                                sText = string.Format("Branch Status {0}", sText);
                                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                                Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left, true, PenWidth.xPen, PenPos.Bottom, PenPos.Bottom);
                            }

                            positionY += height + 10;
                            lineNo++;
                        }
                        #region footer

                        PopulateReportImageFooter(sTmpBranch, sTmpBranchName, userId, "0",
                           ref pageNo, ref totalCheques, ref totalPages,
                           ref xGraphics, ref xRect, ref xTextFormatter,
                           ref positionX, ref positionY, ref width, ref height);
                        #endregion
                    }
                    else
                    {
                        #region reptitle
                        PopulateReportImageHeader(sRepTitle1, sRepTitle2, sRepTitle3,
                                                  totalCheques, pageNo, totalPages,
                                                  sTmpBranch, sTmpBranchName,
                                                  ref xGraphics, ref xRect, ref xTextFormatter,
                                                  ref positionX, ref positionY, ref width, ref height);
                        sText = "";
                        #endregion
                        #region footer
                        PopulateReportImageFooter(sTmpBranch, sTmpBranchName, userId, "0",
                           ref pageNo, ref totalCheques, ref totalPages,
                           ref xGraphics, ref xRect, ref xTextFormatter,
                           ref positionX, ref positionY, ref width, ref height);
                        #endregion
                    }
                    #endregion
                }

                StreamByte = EndWrite(pdfDocument);
                return StreamByte;
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }
        public void PopulateReportImageHeaderMCImages(string sRepTitle1, string sRepTitle2, string sRepTitle3,
                                             int totalCheques, int pageNo, int totalPages,
                                             string sTmpBranch, string sTmpBranchName,
                                       ref XGraphics xGraphics, ref XRect xRect, ref XTextFormatter xTextFormatter,
                                       ref int positionX, ref int positionY, ref int width, ref int height)
        {

            string sText = "";
            #region reptitle
            positionY = 20;
            positionX = 20;
            height = 15;

            width = 960;
            sText = sRepTitle1;
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

            sText = string.Format("Page {0} of {1}", pageNo, totalPages);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Right);
            positionY += height;

            sText = sRepTitle2;
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

            sText = string.Format("{0}", DateTime.Now);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Right);
            positionY += height;

            sText = sRepTitle3;
            if (sText != "")
            {
                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);
                positionY += height;
            }

            sText = string.Format("{0} {1}", sTmpBranch, sTmpBranchName);
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height, true);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

            sText = string.Format("Items: {0}", totalCheques);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Center);

            positionY += height + 10;
            width = 0;
            height = 40;
            #endregion
        }
        public void PopulateReportImageFooterMCImages(string sTmpBranch, string sTmpBranchName,string userId,string TotalItems,
                                       ref int pageNo,ref int totalCheques,ref int totalPages,
                                       ref XGraphics xGraphics, ref XRect xRect, ref XTextFormatter xTextFormatter,
                                       ref int positionX, ref int positionY, ref int width, ref int height)
        {

            string sText = "";
            width = 960;
            positionX = 20;
            sText = string.Format("--- END OF REPORT FOR {0} {1} ---", sTmpBranch, sTmpBranchName);
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);
            positionY += height;

            positionX = 660;
            width = 140;
            //positionY += height;
            height = 8;
            sText = string.Format("Prepared By: {0}", userId);
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left, true, PenWidth.xPen, PenPos.Top, PenPos.Top);

            positionX = 840;
            width = 140;
            sText = "Noted By";
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left, true, PenWidth.xPen, PenPos.Top, PenPos.Top);

            pageNo = 1;

            totalCheques = TotalItems.ToInt();
            totalPages = (totalCheques <= 4) ? 1 : (totalCheques / 4) + ((totalCheques % 4) > 0 ? 1 : 0);
        }
        public void PopulateReportImageHeader(string sRepTitle1, string sRepTitle2, string sRepTitle3,
                                             int totalCheques, int pageNo, int totalPages,
                                             string sTmpBranch, string sTmpBranchName,
                                       ref XGraphics xGraphics, ref XRect xRect, ref XTextFormatter xTextFormatter,
                                       ref int positionX, ref int positionY, ref int width, ref int height)
        {

            string sText = "";
            #region reptitle
            positionY = 20;
            positionX = 20;
            height = 15;

            width = 560;
            sText = sRepTitle1;
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

            sText = string.Format("Page {0} of {1}", pageNo, totalPages);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Right);
            positionY += height;

            sText = sRepTitle2;
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

            sText = string.Format("{0}", DateTime.Now);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Right);
            positionY += height;

            sText = sRepTitle3;
            if (sText != "")
            {
                MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
                Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);
                positionY += height;
            }

            sText = string.Format("{0} {1}", sTmpBranch, sTmpBranchName);
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height, true);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);

            sText = string.Format("Items: {0}", totalCheques);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Center);

            positionY += height + 10;
            width = 0;
            height = 40;
            sText = "";
            #endregion
        }
        public void PopulateReportImageFooter(string sTmpBranch, string sTmpBranchName, string userId, string TotalItems,
                                       ref int pageNo, ref int totalCheques, ref int totalPages,
                                       ref XGraphics xGraphics, ref XRect xRect, ref XTextFormatter xTextFormatter,
                                       ref int positionX, ref int positionY, ref int width, ref int height)
        {
            string sText = "";
            width = 560;
            positionX = 20;
            sText = string.Format("--- END OF REPORT FOR {0} {1} ---",
                sTmpBranch,
                sTmpBranchName
                );
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left);
            positionY += height;

            positionX = 280;
            width = 140;
            positionY += height;
            height = 8;
            sText = string.Format("Prepared By: {0}",
                                userId
                                );
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left, true, PenWidth.xPen, PenPos.Top, PenPos.Top);

            positionX = 440;
            width = 140;
            sText = "Noted By";
            MakeLayoutRect(ref xGraphics, ref xRect, positionX, positionY, width, height);
            Write(ref xGraphics, ref xTextFormatter, ref xRect, sText, FontType.xFontArialBold8, Alignment.Left, true, PenWidth.xPen, PenPos.Top, PenPos.Top);

            pageNo = 1;

            totalCheques = TotalItems.ToInt();
            totalPages = (totalCheques <= 4) ? 1 : (totalCheques / 4) + ((totalCheques % 4) > 0 ? 1 : 0);
        }
        #endregion

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
}
