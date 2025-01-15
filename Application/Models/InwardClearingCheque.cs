using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ParamInwardClearingChequeGetList
    {
        public string? BSRTNFilter { get; set; }
        public DateTime DateFromFilter { get; set; }
        public DateTime DateToFilter { get; set; }
        public string? AccountNumberFilter { get; set; }
        public string? AmountFromFilter { get; set; }
        public string? AmountToFilter { get; set; }
        public string? CheckImageFilter { get; set; }
    }

    public class ParamSaveChequeDetails
    {
        public string? SelectedCheck { get; set; }
        public string? VerifyAction { get; set; }
        public string? SelectedReasonCode { get; set; }
        public string? ReassignReason { get; set; }
        public string? SelectedOfficerCode { get; set; }
        public string? CheckImageStat { get; set; }
    }

    public class CheckDetailModel
    {
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Check Number")]
        public string CheckNumber { get; set; }

        [Display(Name = "Effectivity Date")]
        public DateTime EffectivityDate { get; set; }

        [Display(Name = "Check Status")]
        public string CheckStatus { get; set; }

        [Display(Name = "Reason")]
        public string Reason { get; set; }

        public string VerifiedBy { get; set; }

        public string VerifiedDateTime { get; set; }

        public string ApprovedBy { get; set; }

        public string ApprovedDateTime { get; set; }

        [Display(Name = "Cheque Image File Content")]
        public string ChequeImageFileContent { get; set; }

        [Display(Name = "Cheque Image File Content Rear")]
        public string ChequeImageFileContentR { get; set; }

        [Display(Name = "Cheque Image File Content Front")]
        public string ChequeImageFileContentF { get; set; }

        [Display(Name = "Cheque Image File Content Type")]
        public string ChequeImageFileContentType { get; set; }

        public Dictionary<ChequeStats, string> ChequeStats { get; set; }
    }

    public class ReturnCheckImageDetailTransaction
    {
        public string FileContentType { get; set; }
        public string FileContentTypeR { get; set; }
        public string FileContentTypeF { get; set; }
        public string FileContent { get; set; }
        public string FileContentR { get; set; }
        public string FileContentF { get; set; }
        public string FileName { get; set; }
        public string Amount { get; set; }
        public string CheckStatus { get; set; }
        public string AccountName { get; set; }
        public string AccountStatus { get; set; }
        public string RejectReason { get; set; }
    }

    public class AccountDetails
    {
        public string AccountName { get; set; }
        public string AccountStatus { get; set; }
    }

    public class ReturnViewSignatures
    {
        public List<CustInfo> Info { get; set; }
        public List<ImageView> Images { get; set; }
        public List<SvsGroup> Groups { get; set; }
        public List<SvsRule> Rules { get; set; }
    }

}
