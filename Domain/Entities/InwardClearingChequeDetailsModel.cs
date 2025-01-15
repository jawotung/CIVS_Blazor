using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI;
public partial class InwardClearingChequeDetailsModel
{
    [JsonPropertyName("Id")]
    [Key]
    public int Id { get; set; }
    [JsonPropertyName("accountNumber")]
    [Display(Name = "Account Number"),
        StringLength(12, MinimumLength = 12, ErrorMessage = "Account Number is too short"),
        Required(ErrorMessage = "Account Number is required")]
    public string AccountNumber { get; set; }
    [JsonPropertyName("checkNumber")]

    [Display(Name = "Check Number"),
        StringLength(10, MinimumLength = 10, ErrorMessage = "Check Number is too short"),
        Required(ErrorMessage = "Check Number is required")]
    public string CheckNumber { get; set; }
    [JsonPropertyName("checkAmount")]

    [Display(Name = "Check Amount"), DisplayFormat(DataFormatString = "{0:#,##0.00}"),
    Required(ErrorMessage = "Check Amount is required")]
    public double CheckAmount { get; set; }
    [JsonPropertyName("effectivityDate")]

    [Display(Name = "Effectivity Date")]
    public DateTime EffectivityDate { get; set; }
    [JsonPropertyName("checkStatus")]

    [Display(Name = "Check Status"),
        StringLength(25, MinimumLength = 4, ErrorMessage = "Check Status is too short"),
        Required(ErrorMessage = "Check Status is required")]
    public string CheckStatus { get; set; }
    [JsonPropertyName("reason")]

    [Display(Name = "Reason"),
        StringLength(10)]
    public string? Reason { get; set; } = null;
    [JsonPropertyName("brstn")]

    [Display(Name = "BRSTN"),
        StringLength(10)]
    public string? Brstn { get; set; } = null;
    [JsonPropertyName("branchCode")]

    [Display(Name = "Branch Code"),
        StringLength(10)]
    public string? BranchCode { get; set; } = null;
    [JsonPropertyName("clearingOfficer")]

    [Display(Name = "Clearing Officer"),
        StringLength(50)]
    public string? ClearingOfficer { get; set; } = null;
    [JsonPropertyName("verifiedBy")]

    [Display(Name = "Verified By"),
        StringLength(50)]
    public string? VerifiedBy { get; set; } = null;
    [JsonPropertyName("verifiedDateTime")]

    [Display(Name = "Verified Date")]
    public DateTime VerifiedDateTime { get; set; }
    [JsonPropertyName("approvedBy")]

    [Display(Name = "Approved By"),
        StringLength(50)]
    public string? ApprovedBy { get; set; } = null;
    [JsonPropertyName("approvedDateTime")]

    [Display(Name = "Approved Date")]
    public DateTime ApprovedDateTime { get; set; }
    [JsonPropertyName("chequeImageLinkedKey")]

    public string? ChequeImageLinkedKey { get; set; } = null;
    [JsonPropertyName("checkStatusDisplay")]
    [NotMapped]
    public string CheckStatusDisplay { get; set; }//{ get { return GetStatusDesc(CheckStatus); } }
    [JsonPropertyName("genChequeImageLinkedKey")]

    [NotMapped]
    public string GenChequeImageLinkedKey
    {
        get
        {
            if (CheckNumber != null)
                return string.Format("{0}_{1}_{2}_{3}",
                                        EffectivityDate.ToString("MMddyyyy"),
                                        AccountNumber,
                                        CheckNumber,
                                        CheckAmount);
            else
                return "";
        }
    }
    [JsonPropertyName("reasonDesc")]

    [NotMapped]
    public string ReasonDesc { get; set; }
    [JsonPropertyName("nextSelectedCheck")]

    [NotMapped]
    public string NextSelectedCheck
    {
        get; set;
        //get
        //{
        //    return GetNextChequeImageLinkedKey(ChequeImageLinkedKey, CheckStatus,"", EffectivityDate.ToString(), EffectivityDate.ToString(), BranchCode, ClearingOfficer);
        //}
    }
    [JsonPropertyName("checkBranchOfAccount")]

    [NotMapped]
    public string CheckBranchOfAccount
    {
        get
        {
            return AccountNumber.Substring(0, 3);
        }
    }

    [JsonPropertyName("totalItems")]
    [NotMapped]
    public string TotalItems { get; set; }
    [JsonPropertyName("totalAmount")]

    [NotMapped]
    public string TotalAmount { get; set; }

}
