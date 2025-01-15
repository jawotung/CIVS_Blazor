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
using PdfSharp.Internal;

namespace Infrastructure.Common
{
    public class GeneratePDF
    {
        #region private 

        private static XTextFormatter xTextFormatter;
        private static PdfPage pdfPage;
        private static XRect xRect = new XRect();
        private static XGraphics xGraphics;
        private static readonly XFont xFontRegular6 = new XFont("Courier New", 6, XFontStyleEx.Regular);
        private static readonly XFont xFontRegular8 = new XFont("Courier New", 8, XFontStyleEx.Regular);
        private static readonly XFont xFontRegular10 = new XFont("Courier New", 10, XFontStyleEx.Regular);
        private static readonly XFont xFontRegular12 = new XFont("Courier New", 12, XFontStyleEx.Regular);
        private static readonly XFont xFontBold6 = new XFont("Courier New", 6, XFontStyleEx.Bold);
        private static readonly XFont xFontBold8 = new XFont("Courier New", 8, XFontStyleEx.Bold);
        private static readonly XFont xFontBold10 = new XFont("Courier New", 10, XFontStyleEx.Bold);
        private static readonly XFont xFontBold12 = new XFont("Courier New", 12, XFontStyleEx.Bold);
        private static readonly XFont xFontBold25 = new XFont("Courier New", 25, XFontStyleEx.Bold);
        private static readonly XFont xFontBold35 = new XFont("Courier New", 35, XFontStyleEx.Bold);
        //private static MemoryStream _Stream;// = new MemoryStream();
        private static readonly XFont xFontArialBold6 = new XFont("Arial", 6, XFontStyleEx.Bold);
        private static readonly XFont xFontArialBold8 = new XFont("Arial", 8, XFontStyleEx.Bold);
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
        #endregion

        public enum PenPos
        {
            Top,
            Center,
            Bottom
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

        public enum PenWidth
        {
            xPen,
            xPen2,
            xPen3,
            xPen4
        }

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

        //public MemoryStream Stream;

        public long StreamLength;

        public byte[] StreamByte;



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
        



    }
}
