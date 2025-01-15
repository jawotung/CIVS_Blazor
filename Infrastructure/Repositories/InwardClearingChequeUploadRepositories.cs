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
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class InwardClearingChequeUploadRepositories : IInwardClearingChequeUploadRepository
    {
        private readonly AppDbContext _dBContext;
        private readonly ADClass _adClass;
        private readonly CommonClass _commonClass;
        private readonly GetUserClaims _userClaims;
        private IConfiguration _config;
        public InwardClearingChequeUploadRepositories(AppDbContext dBContext, ADClass addClass, CommonClass commonClass,
            IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _dBContext = dBContext;
            _adClass = addClass;
            _commonClass = commonClass;
            _config = config;
            _userClaims = new GetUserClaims(httpContextAccessor);
        }
        public async Task<ReturnGenericData<InwardClearingChequeUploadReturn>> UploadImage(MCheckImageUpload data)
        {
            ReturnGenericData<InwardClearingChequeUploadReturn> result = new ReturnGenericData<InwardClearingChequeUploadReturn>();
            IFormFile formFile = data.FileUpload.file;
            DateTime sTransactionDate = data.TransactionDate.Value;
            List<InwardClearingChequeDetailsModel> DetailsModelList = new List<InwardClearingChequeDetailsModel>();
            List<InwardClearingChequeImageModel> ImageModelList = new List<InwardClearingChequeImageModel>();
            List<InwardClearingChequeHistoryModel> HistoryModelList = new List<InwardClearingChequeHistoryModel>();
            List<ChequeAccountDetail> AccountModelList = new List<ChequeAccountDetail>();
            try
            {
                BackgroundTaskQueue _queueService = new BackgroundTaskQueue(100, 100); ;
                string errString = "";
                var userClaims = _userClaims.GetClaims();
                string userId = userClaims.UserID;
                string userType = userClaims.UserType;
                string userBranch = userClaims.BranchCode;
                string userBranchBRSTN = _commonClass.GetBranchBRSTN(userBranch);

                string userBranchBuddy = userClaims.BuddyCode;
                string userBranchBuddyName = userClaims.BuddyBranch;
                string userBranchBuddyBRSTN = _commonClass.GetBranchBRSTN(userBranchBuddy);
                if (!formFile.IsNull())
                {
                    string beginUpload = DateTime.Now.ToString();
                    string logMessage = "";
                    string isFileOrDB = _commonClass.GetFileOrDB();
                    string imageFolder = _commonClass.GetImageFolder();
                    string checkImages = _commonClass.GetCheckImages();
                    string fileFormat = _commonClass.GetChequeFileNameFormat();
                    string fileFormatLength = _commonClass.GetChequeFileNameFormatLength();
                    string[] filePrefixFront = _commonClass.GetPrefixFront();
                    string[] filePrefixBack = _commonClass.GetPrefixBack();
                    string[] contentTypeF = _commonClass.GetImageContentTypeContentTypeF();
                    string[] contentTypeR = _commonClass.GetImageContentTypeContentTypeR();

                    bool hasError = false; bool loadDetails = false; bool loadHistory = false; bool loadImage = false;
                    bool fileTypeMatch = false;
                    CheckImageFileName checkImageFileName = new CheckImageFileName(_commonClass);

                    string checkFileName = formFile.FileName;

                    logMessage = string.Format("Uploading {0}.", checkFileName);
                    _commonClass.Log(logMessage);

                    //check if image corrupted
                    if (data.FileUpload.FileByte.IsCorruptedImage())
                    {
                        errString = "Corrupted File Selected. Cannot upload.";
                        logMessage = $"{checkFileName} {errString}";
                        _commonClass.Log(logMessage);
                        return new ReturnGenericData<InwardClearingChequeUploadReturn> { StatusCode = "01", StatusMessage = logMessage };
                    }
                    checkImageFileName.Parse(checkFileName, '_', fileFormat, fileFormatLength);


                    if (filePrefixFront.Contains(checkImageFileName.FrontBack) && contentTypeF.Contains(formFile.ContentType))
                        fileTypeMatch = true;

                    if (filePrefixBack.Contains(checkImageFileName.FrontBack) && contentTypeR.Contains(formFile.ContentType))
                        fileTypeMatch = true;

                    if (!fileTypeMatch)
                    {
                        //hasError = true;
                        errString = "Invalid File Selected. Cannot upload.";
                        logMessage = $"{checkFileName} {errString}";
                        result.StatusMessage = errString;
                        result.StatusCode = "01";

                        //errMsg.Add(logMessage);
                        _commonClass.Log(logMessage);
                        return result;
                    }

                    //check image pattern error
                    if (checkImageFileName.Error.Value)
                    {
                        //hasError = true;
                        errString = "Invalid FileName Format Selected. Cannot upload.";
                        logMessage = $"{checkFileName} {checkImageFileName.ErrorMessage} {errString}";
                        result.StatusMessage = errString;
                        result.StatusCode = "01";

                        //errMsg.Add(logMessage);
                        _commonClass.Log(logMessage);
                        return result;
                    }

                    #region createLinkedKey
                    string effDate = sTransactionDate.ToDate("MMddyyyy");
                    checkImages = checkImages.Replace("[dte]", effDate);
                    checkImages = checkImages.Replace("[bcode]", checkImageFileName.BRSTN);

                    string chequeImageLinkedKey = string.Format("{0}_{1}_{2}_{3}",
                    effDate,
                    checkImageFileName.AccountNumber,
                    checkImageFileName.CheckNumber,
                    checkImageFileName.CheckAmount.GetFloatValue()
                    );
                    string chequeImageLinkedKeyFrontBack = string.Format("{0}_{1}", chequeImageLinkedKey, checkImageFileName.FrontBack);
                    #endregion

                    #region check in history if uploaded today
                    if (await CheckIsUploadedToday(chequeImageLinkedKey))
                    {
                        hasError = true;
                        if (InwardClearingChequeImageModelChequeImageLinkedKeyExists(chequeImageLinkedKeyFrontBack))
                        {
                            errString = "Image already upload today. Cannot upload.";
                            logMessage = $"{checkFileName} {errString}";
                            result.StatusMessage = errString;
                            result.StatusCode = "01";

                            _commonClass.Log(logMessage);
                            return result;
                        }
                        else
                        {
                            hasError = false;
                            loadImage = true;
                        }
                    }
                    else
                    {
                        loadDetails = true;
                        loadHistory = true;
                        loadImage = true;
                    }
                    #endregion

                    if (hasError == false)
                    {
                        if (loadDetails)
                        {
                            InwardClearingChequeDetailsModel inwardClearingChequeDetailsModel = new InwardClearingChequeDetailsModel
                            {
                                AccountNumber = checkImageFileName.AccountNumber,
                                CheckNumber = checkImageFileName.CheckNumber,
                                BranchCode = checkImageFileName.BranchCode,
                                CheckAmount = checkImageFileName.CheckAmount,
                                Brstn = checkImageFileName.BRSTN,
                                EffectivityDate = sTransactionDate, //DateTime.Now,
                                CheckStatus = "Open",
                                Reason = null,
                                VerifiedBy = userId,
                                VerifiedDateTime = sTransactionDate, //DateTime.Now
                                ChequeImageLinkedKey = chequeImageLinkedKey,
                                ReasonDesc = "",
                                TotalItems = "",
                                TotalAmount = "",
                                NextSelectedCheck = "",
                                CheckStatusDisplay = "",
                            };
                            DetailsModelList.Add(inwardClearingChequeDetailsModel);

                            string[] sParams = new string[]
                            {
                                 inwardClearingChequeDetailsModel.ChequeImageLinkedKey,
                                 "Upload",
                                 inwardClearingChequeDetailsModel.CheckStatus,
                                 inwardClearingChequeDetailsModel.Reason,
                                 inwardClearingChequeDetailsModel.BranchCode,
                                 ""
                            };
                            if (loadHistory)
                            {
                                InwardClearingChequeHistoryModel inwardClearingChequeHistory = new InwardClearingChequeHistoryModel
                                {
                                    ChequeImageLinkedKey = sParams[0],
                                    CheckStatusFrom = sParams[1],
                                    CheckStatusTo = sParams[2],
                                    Reason = sParams[3],
                                    BranchCode = sParams[4],
                                    ClearingOfficer = sParams[5],
                                    ActionBy = userId,
                                    ActionDateTime = DateTime.Now
                                };

                                HistoryModelList.Add(inwardClearingChequeHistory);
                            }

                            //add account status below
                            ChequeAccountDetail chequeAccountDetails;
                            if ((inwardClearingChequeDetailsModel.CheckBranchOfAccount == "000"))
                            {
                                chequeAccountDetails = new ChequeAccountDetail
                                {
                                    ChequeImageLinkedKey = chequeImageLinkedKey,
                                    AccountNumber = checkImageFileName.AccountNumber,
                                    AccountName = "Managers Check",
                                    AccountStatus = "",
                                    EffectivityDate = inwardClearingChequeDetailsModel.EffectivityDate,
                                    StatusAsOfDate = DateTime.Now
                                };
                                AccountModelList.Add(chequeAccountDetails);
                            }
                            else
                            {
                                chequeAccountDetails = new ChequeAccountDetail
                                {
                                    ChequeImageLinkedKey = chequeImageLinkedKey,
                                    AccountNumber = checkImageFileName.AccountNumber,
                                    EffectivityDate = inwardClearingChequeDetailsModel.EffectivityDate,
                                    AccountStatus = "ForUpdating",
                                    AccountName = "ForUpdating",
                                    StatusAsOfDate = DateTime.Now
                                };
                                AccountModelList.Add(chequeAccountDetails);
                            }

                        }

                        if (loadImage)
                        {

                            checkImages = checkImages.Replace("[dte]", effDate);
                            checkImages = checkImages.Replace("[bcode]", checkImageFileName.BRSTN);

                            InwardClearingChequeImageModel inwardClearingChequeImageModel = new InwardClearingChequeImageModel
                            {
                                ChequeImageLinkedKey = chequeImageLinkedKeyFrontBack,
                                ChequeImageFileContent = isFileOrDB == "File" ? checkImages : data.FileUpload.FileByte.ConvertToPngBase64(),
                                ChequeImageFileContentType = "image/png",
                                ChequeImageFileName = checkFileName
                            };

                            if (isFileOrDB == "File")
                            {
                                using (var memoryStream = new MemoryStream(data.FileUpload.FileByte))
                                {
                                    _commonClass.SaveImage(memoryStream, checkImages, inwardClearingChequeImageModel.ChequeImageLinkedKey);
                                }
                            }
                            ImageModelList.Add(inwardClearingChequeImageModel);
                        }

                        result.Data = new InwardClearingChequeUploadReturn
                        {
                            DetailsModelList = DetailsModelList,
                            ImageModelList = ImageModelList,
                            HistoryModelList = HistoryModelList,
                            AccountModelList = AccountModelList,
                        };
                        result.StatusMessage = "Sent.";
                        result.StatusCode = "00";

                        logMessage = string.Format("{0} Upload Queued.", checkFileName);
                        _commonClass.Log(logMessage);
                    }
                    else
                    {
                        result.StatusCode = "01";
                        result.StatusMessage = "Queued Error.";
                        logMessage = string.Format("{0} Uploading Error.", checkFileName);
                        _commonClass.Log(logMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ReturnGenericData<InwardClearingChequeUploadReturn> { StatusCode = "01", StatusMessage = ex.Message };
            }

            return result;
        }
        public async Task<ReturnGenericData<MReturnSaveImage>> SaveImage(InwardClearingChequeUploadReturn data)
        {
            ReturnGenericData<MReturnSaveImage> result = new ReturnGenericData<MReturnSaveImage>();
            bool save = true;
            ReturnGenericStatus DetailsResult = await SaveDetails(data.DetailsModelList);
            ReturnGenericStatus HistoryResult = await SaveHistory(data.HistoryModelList);
            ReturnGenericStatus ImageResult = await SaveImages(data.ImageModelList);
            ReturnGenericStatus AccountResult = await SaveAccount(data.AccountModelList);
            if (DetailsResult.StatusCode == "00" && HistoryResult.StatusCode == "00" && ImageResult.StatusCode == "00" && AccountResult.StatusCode == "00")
            {
                await _dBContext.SaveChangesAsync();
                data.DetailsModelList.ForEach(o => _commonClass.Log(string.Format("{0} Saved.", o.ChequeImageLinkedKey)));
                result.StatusCode = "00";
                result.StatusMessage = "File Saved: " + data.ImageModelList.Count;
            }
            else
            {
                result.StatusCode = "01";
                result.StatusMessage = "Failed to save the files.";
            }
            result.Data = new MReturnSaveImage
            {
                DetailsResult = DetailsResult,
                HistoryResult = HistoryResult,
                ImageResult = ImageResult,
                AccountResult = AccountResult,
            };
            return result;
        }
        private async Task<ReturnGenericStatus> SaveDetails(List<InwardClearingChequeDetailsModel> DetailsModelList)
        {
            ReturnGenericStatus uploadStatus = new ReturnGenericStatus() { StatusCode = "01", StatusMessage = "Error Saving" };
            if (DetailsModelList.Count > 0)
            {
                Console.WriteLine("Total Details before: {0}", DetailsModelList.Count);
                //DetailsModelList.ForEach(a => Console.WriteLine(a.ChequeImageLinkedKey)); 
                var uploadDetailsList = DetailsModelList.GroupBy(d => d.ChequeImageLinkedKey).Select(dm => dm.First()).ToList();
                Console.WriteLine("Total Details after: {0}", uploadDetailsList.Count);
                //uploadDetailsList.ForEach(a => Console.WriteLine(a.ChequeImageLinkedKey));
                try
                {
                    DetailsModelList = uploadDetailsList;
                    foreach (var item in DetailsModelList)
                    {
                        if (await _dBContext.InwardClearingChequeDetailsModels.AnyAsync(cad => cad.ChequeImageLinkedKey == item.ChequeImageLinkedKey))
                        {
                            uploadDetailsList.Remove(item);
                        }
                    }
                    if (uploadDetailsList.Count > 0)
                    {
                        await _dBContext.AddRangeAsync(uploadDetailsList);
                        //await _context.SaveChangesAsync();
                    }
                    Console.WriteLine("Total Details Saved: {0}", uploadDetailsList.Count);
                    Console.WriteLine("Details Saved End: {0}", DateTime.Now);
                    uploadStatus.StatusCode = "00";
                    uploadStatus.StatusMessage = uploadDetailsList.Count > 0 ? "Saved." : "Nothing to save";
                    DetailsModelList.ForEach(o => _commonClass.Log(string.Format("{0} Saved.", o.ChequeImageLinkedKey)));
                }
                catch (Exception ex)
                {
                    uploadStatus.StatusCode = "01";
                    uploadStatus.StatusMessage = ex.Message;
                    Console.WriteLine("Details Saved Error: {0} {1}", ex.Message, DateTime.Now);
                    _commonClass.Log(ex.Message);
                }
            }
            else
            {
                uploadStatus.StatusCode = "01";
                uploadStatus.StatusMessage = "Nothing to save";
            }
            return uploadStatus;
        }
        private async Task<ReturnGenericStatus> SaveHistory(List<InwardClearingChequeHistoryModel> HistoryModelList)
        {
            ReturnGenericStatus uploadStatus = new ReturnGenericStatus() { StatusCode = "01", StatusMessage = "Error Saving" };
            if (HistoryModelList.Count > 0)
            {
                Console.WriteLine("Total History before: {0}", HistoryModelList.Count);
                //HistoryModelList.ForEach(a => Console.WriteLine(a.ChequeImageLinkedKey));
                var uploadDetailsList = HistoryModelList.GroupBy(d => d.ChequeImageLinkedKey).Select(dm => dm.First()).ToList();
                Console.WriteLine("Total History after: {0}", uploadDetailsList.Count);
                //uploadDetailsList.ForEach(a => Console.WriteLine(a.ChequeImageLinkedKey));

                try
                {
                    HistoryModelList = uploadDetailsList;
                    Console.WriteLine("History Saved Begin: {0}", DateTime.Now);
                    foreach (var item in HistoryModelList)
                    {
                        if (await _dBContext.InwardClearingChequeHistoryModels.AnyAsync(cad => cad.ChequeImageLinkedKey == item.ChequeImageLinkedKey))
                        {
                            uploadDetailsList.Remove(item);
                        }
                    }
                    if (uploadDetailsList.Count > 0)
                    {
                        await _dBContext.AddRangeAsync(uploadDetailsList);
                        //await _context.SaveChangesAsync();
                    }
                    Console.WriteLine("Total History Saved: {0}", uploadDetailsList.Count);
                    Console.WriteLine("History Saved End: {0}", DateTime.Now);
                    uploadStatus.StatusCode = "00";
                    uploadStatus.StatusMessage = uploadDetailsList.Count > 0 ? "Saved." : "Nothing to save";
                }
                catch (Exception ex)
                {
                    uploadStatus.StatusCode = "01";
                    uploadStatus.StatusMessage = ex.Message;
                    Console.WriteLine("History Saved Error: {0} {1}", ex.Message, DateTime.Now);
                    _commonClass.Log(ex.Message);
                }
            }
            else
            {
                uploadStatus.StatusCode = "01";
                uploadStatus.StatusMessage = "Nothing to save";
            }

            return uploadStatus;
        }
        private async Task<ReturnGenericStatus> SaveImages(List<InwardClearingChequeImageModel> ImageModelList)
        {
            ReturnGenericStatus uploadStatus = new ReturnGenericStatus() { StatusCode = "01", StatusMessage = "Error Saving" };
            if (ImageModelList.Count > 0)
            {
                Console.WriteLine("Total Image before: {0}", ImageModelList.Count);
                //ImageModelList.ForEach(a => Console.WriteLine(a.ChequeImageLinkedKey));
                var uploadDetailsList = ImageModelList.GroupBy(d => d.ChequeImageLinkedKey).Select(dm => dm.First()).ToList();
                Console.WriteLine("Total Image after: {0}", uploadDetailsList.Count);
                //uploadDetailsList.ForEach(a => Console.WriteLine(a.ChequeImageLinkedKey));

                try
                {
                    ImageModelList = uploadDetailsList;
                    Console.WriteLine("Image Saved Begin: {0}", DateTime.Now);
                    foreach (var item in ImageModelList)
                    {
                        if (await _dBContext.InwardClearingChequeImageModels.AnyAsync(cad => cad.ChequeImageLinkedKey == item.ChequeImageLinkedKey))
                        {
                            uploadDetailsList.Remove(item);
                        }
                    }
                    if (uploadDetailsList.Count > 0)
                    {
                        await _dBContext.AddRangeAsync(uploadDetailsList);
                        //await _context.SaveChangesAsync();
                    }
                    Console.WriteLine("Total Image Saved: {0}", uploadDetailsList.Count);
                    Console.WriteLine("Image Saved End: {0}", DateTime.Now);
                    uploadStatus.StatusCode = "00";
                    uploadStatus.StatusMessage = uploadDetailsList.Count > 0 ? "Saved." : "Nothing to save";
                }
                catch (Exception ex)
                {
                    //uploadStatus.DetailsCount = 1;
                    uploadStatus.StatusCode = "01";
                    uploadStatus.StatusMessage = ex.Message;
                    Console.WriteLine("Image Saved Error: {0} {1}", ex.Message, DateTime.Now);
                    _commonClass.Log(ex.Message);
                }
            }
            else
            {
                uploadStatus.StatusCode = "01";
                uploadStatus.StatusMessage = "Nothing to save";
            }

            return uploadStatus;
        }
        private async Task<ReturnGenericStatus> SaveAccount(List<ChequeAccountDetail> AccountModelList)
        {
            ReturnGenericStatus uploadStatus = new ReturnGenericStatus { StatusCode= "01", StatusMessage = "Error Saving" };
            if (AccountModelList.Count > 0)
            {
                Console.WriteLine("Total Account before: {0}", AccountModelList.Count);
                //AccountModelList.ForEach(a => Console.WriteLine(a.ChequeImageLinkedKey));
                var uploadDetailsList = AccountModelList.GroupBy(d => d.ChequeImageLinkedKey).Select(dm => dm.First()).ToList();
                Console.WriteLine("Total Account after: {0}", uploadDetailsList.Count);
                //uploadDetailsList.ForEach(a => Console.WriteLine(a.ChequeImageLinkedKey, a.AccountNumber));
                try
                {
                    //var uploadDetailsList = new List<ChequeAccountDetails>();
                    AccountModelList = uploadDetailsList.ToList<ChequeAccountDetail>();
                    Console.WriteLine("Account Saved Begin: {0}", DateTime.Now);
                    foreach (var item in AccountModelList)
                    {
                        if (await _dBContext.ChequeAccountDetails.AnyAsync(cad => cad.ChequeImageLinkedKey == item.ChequeImageLinkedKey))
                        {
                            uploadDetailsList.Remove(item);
                        }
                    }
                    if (uploadDetailsList.Count > 0)
                    {
                        var res = uploadDetailsList.Select(a => a.AccountNumber).ToList().GetAccountNameStatusPostGre(_config);
                        foreach (var item in res)
                        {
                            uploadDetailsList.ForEach(u =>
                            {
                                if (u.AccountNumber == item.AccountNumber)
                                {
                                    u.AccountName = item.AccountName;
                                    u.AccountStatus = item.AccountStatus;
                                }
                            }
                            );
                        }
                        await _dBContext.AddRangeAsync(uploadDetailsList);
                        //await _context.SaveChangesAsync();
                    }
                    Console.WriteLine("Total Account Saved: {0}", uploadDetailsList.Count);
                    Console.WriteLine("Account Saved End: {0}", DateTime.Now);
                    uploadStatus.StatusCode = "00";
                    uploadStatus.StatusMessage = uploadDetailsList.Count > 0 ? "Saved." : "Nothing to save";
                }
                catch (Exception ex)
                {
                    uploadStatus.StatusCode = "01";
                    uploadStatus.StatusMessage = ex.Message;
                    Console.WriteLine("Account Saved Error: {0} {1}", ex.Message, DateTime.Now);
                    _commonClass.Log(ex.Message);
                }
            }
            else
            {
                uploadStatus.StatusCode = "01";
                uploadStatus.StatusMessage = "Nothing to save";
            }

            return uploadStatus;
        }
        private bool InwardClearingChequeImageModelChequeImageLinkedKeyExists(string chequeImageLinkedKey)
        {
            return _dBContext.InwardClearingChequeImageModels.Any(e => e.ChequeImageLinkedKey == chequeImageLinkedKey);
        }
        private async Task<bool> CheckIsUploadedToday(string chequeImageLinkedKey)
        {
            return await _dBContext.InwardClearingChequeHistoryModels
                .AnyAsync(h =>
                h.ChequeImageLinkedKey == chequeImageLinkedKey
                && h.CheckStatusFrom == "Upload"
                && h.ActionDateTime.Date == DateTime.Today);
        }

    }
    public class CheckImageFileName
    {
        private readonly CommonClass _commonClass;
        public CheckImageFileName(CommonClass commonClass)
        {
            _commonClass = commonClass;
            Error = false;
        }

        public string AccountNumber { get; set; } = string.Empty;

        public string CheckNumber { get; set; } = string.Empty;

        public string BranchCode { get; set; } = string.Empty;

        public string BRSTN { get; set; } = string.Empty;

        public double CheckAmount { get; set; } = 0.00;

        public string FrontBack { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public bool? Error { get; set; }
        public void ParseV1(string sFile, char cPattern)
        {
            sFile = sFile.GetFileWOExt();

            if (!sFile.Contains(cPattern))
            {
                Error = true;
                ErrorMessage = sFile + " File name pattern error.";
            }
            else
            {
                var vFileName = sFile.Split(cPattern);
                double oChkAmt;// = 0;

                switch (vFileName.Length)
                {
                    case 1:
                        Error = true;
                        ErrorMessage = sFile + " File name pattern error.";
                        break;
                    case 2:
                        AccountNumber = vFileName[0];
                        CheckNumber = vFileName[1];
                        BranchCode = AccountNumber.Length > 3 ? AccountNumber.Substring(0, 3) : "";
                        CheckAmount = 0.00;
                        break;
                    case 3:
                        AccountNumber = vFileName[0];
                        CheckNumber = vFileName[1];
                        BranchCode = AccountNumber.Length > 3 ? AccountNumber.Substring(0, 3) : "";
                        CheckAmount = double.TryParse(vFileName[2], out oChkAmt) == true ? oChkAmt : oChkAmt;
                        break;
                    default:
                        Error = true;
                        ErrorMessage = sFile + " File name pattern error.";
                        break;
                }
            }
        }

        public void Parse(string sFile, char cPattern, string sFileFormat, string sFileFormatLength)
        {
            Error = false; ErrorMessage = "";
            sFile = sFile.GetFileWOExt();

            if (!sFile.Contains(cPattern))
            {
                Error = true;
                ErrorMessage = sFile + " File name pattern error.";
            }
            else
            {
                var vFileFormat = sFileFormat.Split(cPattern);
                var vFileFormatLength = sFileFormatLength.Split(cPattern);
                Dictionary<string, string> dFileName = MakeDictionary(vFileFormat);

                var vFileName = sFile.Split(cPattern);
                double oChkAmt = 0;

                switch (vFileName.Length)
                {
                    case 5:
                        //check length
                        for (int iLoop = 0; iLoop < vFileFormatLength.Length; iLoop++)
                        {
                            if (vFileName[iLoop].GetValue().Length != vFileFormatLength[iLoop].GetValue().ToInt())
                            {
                                Error = true;
                                ErrorMessage = sFile + " File name pattern error.";
                                break;
                            }
                        }

                        if (Error == false)
                        {
                            dFileName[vFileFormat[0].GetValue()] = vFileName[0].GetValue();
                            dFileName[vFileFormat[1].GetValue()] = vFileName[1].GetValue();
                            dFileName[vFileFormat[2].GetValue()] = vFileName[2].GetValue();
                            dFileName[vFileFormat[3].GetValue()] = vFileName[3].GetValue();
                            dFileName[vFileFormat[4].GetValue()] = vFileName[4].GetValue();
                        }
                        break;
                    default:
                        Error = true;
                        ErrorMessage = sFile + " File name pattern error.";
                        break;
                }

                switch (Error)
                {
                    case false:
                        AccountNumber = dFileName.ContainsKey("AccountNumber") ? dFileName["AccountNumber"].GetValue() : "";
                        CheckNumber = dFileName.ContainsKey("CheckNumber") ? dFileName["CheckNumber"].GetValue() : "";
                        CheckAmount = dFileName.ContainsKey("CheckAmount") ? dFileName["CheckAmount"].ToDouble() : oChkAmt;
                        BRSTN = dFileName.ContainsKey("BRSTN") ? dFileName["BRSTN"].GetValue() : "";
                        FrontBack = dFileName.ContainsKey("FrontBack") ? dFileName["FrontBack"].GetValue() : "";
                        BranchCode = _commonClass.GetBranchOfAssignment(BRSTN);
                        break;
                }
            }

        }
        private Dictionary<string, string> MakeDictionary(string[] sKeys)
        {
            Dictionary<string, string> dResult = new Dictionary<string, string> { };

            foreach (string sKey in sKeys)
                dResult.TryAdd(sKey, "");

            return dResult;
        }
    }
}
