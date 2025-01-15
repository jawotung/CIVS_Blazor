using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class MCCheckDetailModel
    {
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Check Number")]
        public string CheckNumber { get; set; }

        [Display(Name = "Check Amount")]
        public string CheckAmount { get; set; }

        [Display(Name = "Branch BRSTN")]
        public string BRSTN { get; set; }

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
}
