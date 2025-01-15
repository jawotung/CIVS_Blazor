using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using Application.Models;

public class MCheckImageUpload
{
    [NotMapped,
        Required(ErrorMessage = "Pick an Image to save."),
        Display(Name = "Upload Check Image File")]
    [JsonPropertyName("fileUpload")]
    public ClientFormFile FileUpload { get; set; }

    [JsonPropertyName("fileUploadRes")]
    public string FileUploadRes { get; set; } = string.Empty;

    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Pick a Transaction Date."),
        Display(Name = "Transaction Date"),
        DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
    [JsonPropertyName("transactionDate")]
    public DateTime? TransactionDate { get; set; }
}

public class CheckImageFileName
{
    //private readonly CommonClass _commonClass;
    public CheckImageFileName()
    {
        Error = false;
        //_commonClass = commonClass;
    }
    public string AccountNumber { get; set; }
    public string CheckNumber { get; set; }
    public string BranchCode { get; set; }
    public string Brstn { get; set; }
    public double CheckAmount { get; set; }
    public string FrontBack { get; set; }
    public string ErrorMessage { get; set; }
    public bool Error { get; set; }
    //public void ParseV1(string sFile, char cPattern)
    //{
    //    sFile = sFile.GetFileWOExt();

    //    if (!sFile.Contains(cPattern))
    //    {
    //        Error = true;
    //        ErrorMessage = sFile + " File name pattern error.";
    //    }
    //    else
    //    {
    //        var vFileName = sFile.Split(cPattern);
    //        double oChkAmt;// = 0;

    //        switch (vFileName.Length)
    //        {
    //            case 1:
    //                Error = true;
    //                ErrorMessage = sFile + " File name pattern error.";
    //                break;
    //            case 2:
    //                AccountNumber = vFileName[0];
    //                CheckNumber = vFileName[1];
    //                BranchCode = AccountNumber.Length > 3 ? AccountNumber.Substring(0, 3) : "";
    //                CheckAmount = 0.00;
    //                break;
    //            case 3:
    //                AccountNumber = vFileName[0];
    //                CheckNumber = vFileName[1];
    //                BranchCode = AccountNumber.Length > 3 ? AccountNumber.Substring(0, 3) : "";
    //                CheckAmount = double.TryParse(vFileName[2], out oChkAmt) == true ? oChkAmt : oChkAmt;
    //                break;
    //            default:
    //                Error = true;
    //                ErrorMessage = sFile + " File name pattern error.";
    //                break;
    //        }
    //    }
    //}
    //public void Parse(string sFile, char cPattern, string sFileFormat, string sFileFormatLength)
    //{
    //    Error = false; ErrorMessage = "";
    //    sFile = sFile.GetFileWOExt();

    //    if (!sFile.Contains(cPattern))
    //    {
    //        Error = true;
    //        ErrorMessage = sFile + " File name pattern error.";
    //    }
    //    else
    //    {
    //        var vFileFormat = sFileFormat.Split(cPattern);
    //        var vFileFormatLength = sFileFormatLength.Split(cPattern);
    //        Dictionary<string, string> dFileName = MakeDictionary(vFileFormat);

    //        var vFileName = sFile.Split(cPattern);
    //        double oChkAmt = 0;

    //        switch (vFileName.Length)
    //        {
    //            //case 1:
    //            //    Error = true;
    //            //    ErrorMessage = sFile + " File name pattern error.";
    //            //    break;
    //            //case 2:
    //            //    dFileName[vFileFormat[0].GetValue()] = vFileName[0].GetValue();
    //            //    dFileName[vFileFormat[1].GetValue()] = vFileName[1].GetValue(); 
    //            //    break;
    //            //case 3:
    //            //    dFileName[vFileFormat[0].GetValue()] = vFileName[0].GetValue();
    //            //    dFileName[vFileFormat[1].GetValue()] = vFileName[1].GetValue();
    //            //    dFileName[vFileFormat[2].GetValue()] = vFileName[2].GetValue(); 
    //            //    break;
    //            //case 4:
    //            case 5:
    //                //check length
    //                for (int iLoop = 0; iLoop < vFileFormatLength.Length; iLoop++)
    //                {
    //                    if (vFileName[iLoop].GetValue().Length != vFileFormatLength[iLoop].GetValue().ToInt())
    //                    {
    //                        Error = true;
    //                        ErrorMessage = sFile + " File name pattern error.";
    //                        break;
    //                    }
    //                }

    //                if (Error == false)
    //                {
    //                    dFileName[vFileFormat[0].GetValue()] = vFileName[0].GetValue();
    //                    dFileName[vFileFormat[1].GetValue()] = vFileName[1].GetValue();
    //                    dFileName[vFileFormat[2].GetValue()] = vFileName[2].GetValue();
    //                    dFileName[vFileFormat[3].GetValue()] = vFileName[3].GetValue();
    //                    dFileName[vFileFormat[4].GetValue()] = vFileName[4].GetValue();
    //                }
    //                break;
    //            default:
    //                Error = true;
    //                ErrorMessage = sFile + " File name pattern error.";
    //                break;
    //        }

    //        switch (Error)
    //        {
    //            case false:
    //                AccountNumber = dFileName.ContainsKey("AccountNumber") ? dFileName["AccountNumber"].GetValue() : "";
    //                CheckNumber = dFileName.ContainsKey("CheckNumber") ? dFileName["CheckNumber"].GetValue() : "";
    //                CheckAmount = dFileName.ContainsKey("CheckAmount") ? dFileName["CheckAmount"].ToDouble() : oChkAmt;
    //                Brstn = dFileName.ContainsKey("BRSTN") ? dFileName["BRSTN"].GetValue() : "";
    //                FrontBack = dFileName.ContainsKey("FrontBack") ? dFileName["FrontBack"].GetValue() : "";
    //                BranchCode = _commonClass.GetBranchOfAssignment(Brstn);
    //                break;
    //        }
    //    }

    //}
    private Dictionary<string, string> MakeDictionary(string[] sKeys)
    {
        Dictionary<string, string> dResult = new Dictionary<string, string> { };

        foreach (string sKey in sKeys)
            dResult.TryAdd(sKey, "");

        return dResult;
    }

}
//public class ClientFormFile : IFormFile
//{
//    public string ContentType { get; set; }
//    public string ContentDisposition { get; set; }
//    public IHeaderDictionary Headers { get; set; }
//    public long Length { get; set; }
//    public string Name { get; set; }
//    public string FileName { get; set; }

//    public void CopyTo(Stream target)
//    {
//        throw new NotImplementedException();
//    }

//    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }

//    public Stream OpenReadStream()
//    {
//        // Implement this method to return a stream with the file content
//        return new MemoryStream();
//    }
//}
public class ClientFormFile
{
    public string FileName { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
    public byte[] FileByte { get; set; }
    public IFormFile? file { get; set; }
    public string? Status { get; set; }
    public string? ButtonStatus { get; set; }
    public string? Msg { get; set; }
    public string? MsgStatus { get; set; }
}

public class MReturnSaveImage
{
    public ReturnGenericStatus DetailsResult { get; set; }
    public ReturnGenericStatus HistoryResult { get; set; }
    public ReturnGenericStatus ImageResult { get; set; }
    public ReturnGenericStatus AccountResult { get; set; }
}