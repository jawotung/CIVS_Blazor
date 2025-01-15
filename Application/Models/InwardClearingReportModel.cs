using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{

    public enum ChequeStats
    {
        Upload,
        Accept,
        Reject,
        ReAssign,
        ReferToOfficer,
        BrAccept,
        BrReject,
        COAccept,
        COReject,
        COReAssign
    }
    public class InwardClearingReportModel
    {

        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Check Number")]
        public string CheckNumber { get; set; }

        [Display(Name = "Check Amount"), DisplayFormat(DataFormatString = "{0:#,##0.00}")]
        public double CheckAmount { get; set; }

        [Display(Name = "Effectivity Date")]
        public string EffectivityDate { get; set; }

        [Display(Name = "Check Status")]
        public string CheckStatus { get; set; }

        [Display(Name = "Reject Reason")]
        public string RejectReason { get; set; }

        [Display(Name = "ReAssign Reason")]
        public string ReAssignReason { get; set; }

        [Display(Name = "Branch Code")]
        public string BranchCode { get; set; }

        [Display(Name = "Next level Approver")]
        public string ClearingOfficer { get; set; }

        [Display(Name = "Verified By")]
        public string VerifiedBy { get; set; }

        [Display(Name = "Verified Date")]
        public string VerifiedDateTime { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Approved Date")]
        public string ApprovedDateTime { get; set; }

        [Display(Name = "Reassigned By")]
        public string ReassignedBy { get; set; }

        [Display(Name = "Reassigned Date")]
        public string ReassignedDateTime { get; set; }

        [Display(Name = "Total Items")]
        public string TotalItems { get; set; }

        [Display(Name = "Total Amount")]
        public string TotalAmount { get; set; }

        public string FrontImage { get; set; }

        public string BackImage { get; set; }

        public string BranchName { get; set; }

        public string BranchBRSTN { get; set; }

        public Dictionary<ChequeStats, string> ChequeStats { get; set; }
    }
}
