using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Models
{
    public class CheckVerifyModel
    {
        [Display(Name = "Account Number")]
        public string AccountNumberFilter { get; set; }

        [Display(Name = "Check Status")]
        public string CheckImageFilter { get; set; }

        [Display(Name = "BRSTN")]
        public string BSRTNFilter { get; set; }

        [Display(Name = "Amount From")]
        public string AmountFromFilter { get; set; }

        [Display(Name = "Amount To"), NUmberGreaterThanOrEqualAttribute("AmountFromFilter", ErrorMessage = "Amount to must be greater than Amount from")]
        public string AmountToFilter { get; set; }

        [Display(Name = "Date From")]
        public DateTime DateFromFilter { get; set; } = DateTime.Now;

        [Display(Name = "Date To"), DateGreaterThanOrEqual("DateFromFilter", ErrorMessage = "Date to must be greater than Date from")]
        public DateTime DateToFilter { get; set; } = DateTime.Now;
        [Display(Name = "Action"),
            RequiredIf("SelectedCheck", "IsNotNull", "Action is Required")]
        public string VerifyAction { get; set; }

        [Display(Name = "SelectedCheck")]
        public string SelectedCheck { get; set; }

        [Display(Name = "NextSelectedCheck")]
        public string NextSelectedCheck { get; set; }

        public string CheckImageStat { get; set; }

        public string CheckImageSource { get; set; }

        [Display(Name = "Re-Assign to Branch")]
        //RequiredIf("VerifyAction", "ReAssign", "Branch is Required")]
        public string SelectedBranchCode { get; set; }
        [Display(Name = "Reason for Re-Assign to Branch"),
            RequiredIf("VerifyAction", "ReAssign", "Re-Assign Reason is Required")]
        public string ReassignReason { get; set; }
        [Display(Name = "Reject Reason"),
            RequiredIf("VerifyAction", "Reject", "Reject Reason is Required.")]
        public string SelectedReasonCode { get; set; }
        [Display(Name = "Next Level Appover"),
            RequiredIf("VerifyAction", "ReferToOfficer", "Officer is Required.")]
        public string SelectedOfficerCode { get; set; }

        public InwardClearingChequeDetailsModel InwardClearingChequeHeader { get; set; }

        public PaginatedList<InwardClearingChequeDetailsModel> InwardClearingChequeDetails { get; set; }
    }
}
