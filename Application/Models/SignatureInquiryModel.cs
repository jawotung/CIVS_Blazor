using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ParamSignatureInquiry
    {
        [JsonPropertyName("accountid")]
        [StringLength(100, ErrorMessage = "AccountId cannot exceed 100 characters.")]
        public string AccountId { get; set; }

        [JsonPropertyName("accountType")]
        [StringLength(50, ErrorMessage = "AccountType cannot exceed 50 characters.")]
        public string AccountType { get; set; }

        [JsonPropertyName("bankCode")]
        [StringLength(8, ErrorMessage = "BankCode cannot exceed 8 characters.")]
        public string BankCode { get; set; }

        [JsonPropertyName("crDb")]
        [StringLength(10, ErrorMessage = "CrDb cannot exceed 10 characters.")]
        public string CrDb { get; set; }

        [JsonPropertyName("customerId")]
        [StringLength(100, ErrorMessage = "CustomerId cannot exceed 100 characters.")]
        public string CustomerId { get; set; }

        [JsonPropertyName("tranAmount")]
        public TransactionAmount TranAmount { get; set; }

        [JsonPropertyName("isFullImage")]
        [StringLength(1, ErrorMessage = "IsFullImage cannot exceed 1 character.")]
        public string IsFullImage { get; set; }
    }

    public class TransactionAmount
    {
        [JsonPropertyName("amountValue")]
        public double AmountValue { get; set; }

        [JsonPropertyName("currencyCode")]
        [StringLength(3, ErrorMessage = "CurrencyCode cannot exceed 3 characters.")]
        public string CurrencyCode { get; set; }
    }

    public class ReturnSignatureInquiry
    {
        [JsonPropertyName("ruleData")]
        public List<RuleData> RuleData { get; set; }

        [JsonPropertyName("signatureData")]
        public List<SignatureData> SignatureData { get; set; }
    }

    public class RuleData
    {
        [JsonPropertyName("ruleDesc")]
        [StringLength(500, ErrorMessage = "RuleDesc cannot exceed 500 characters.")]
        public string RuleDesc { get; set; }

        [JsonPropertyName("ruleName")]
        [StringLength(100, ErrorMessage = "RuleName cannot exceed 100 characters.")]
        public string RuleName { get; set; }
    }

    public class SignatureData
    {
        [JsonPropertyName("isActive")]
        [StringLength(1, ErrorMessage = "IsActive cannot exceed 1 character.")]
        public string IsActive { get; set; }

        [JsonPropertyName("isExpired")]
        [StringLength(1, ErrorMessage = "IsExpired cannot exceed 1 character.")]
        public string IsExpired { get; set; }

        [JsonPropertyName("isMandatory")]
        [StringLength(1, ErrorMessage = "IsMandatory cannot exceed 1 character.")]
        public string IsMandatory { get; set; }

        [JsonPropertyName("isViewRestricted")]
        [StringLength(1, ErrorMessage = "IsViewRestricted cannot exceed 1 character.")]
        public string IsViewRestricted { get; set; }

        [JsonPropertyName("remarks")]
        [StringLength(650, ErrorMessage = "Remarks cannot exceed 650 characters.")]
        public string Remarks { get; set; }

        [JsonPropertyName("returnedSignature")]
        public string ReturnedSignature { get; set; }

        [JsonPropertyName("photoIsMandatory")]
        [StringLength(1, ErrorMessage = "PhotoIsMandatory cannot exceed 1 character.")]
        public string PhotoIsMandatory { get; set; }

        [JsonPropertyName("returnedPhotograph")]
        public string ReturnedPhotograph { get; set; }

        [JsonPropertyName("custName")]
        [StringLength(250, ErrorMessage = "CustName cannot exceed 250 characters.")]
        public string CustName { get; set; }

        [JsonPropertyName("solId")]
        [StringLength(50, ErrorMessage = "SolId cannot exceed 50 characters.")]
        public string SolId { get; set; }

        [JsonPropertyName("keyword")]
        [StringLength(50, ErrorMessage = "Keyword cannot exceed 50 characters.")]
        public string Keyword { get; set; }

        [JsonPropertyName("signId")]
        [StringLength(30, ErrorMessage = "SignId cannot exceed 30 characters.")]
        public string SignId { get; set; }

        [JsonPropertyName("signGpName")]
        [StringLength(100, ErrorMessage = "SignGpName cannot exceed 100 characters.")]
        public string SignGpName { get; set; }

        [JsonPropertyName("signEffDt")]
        public DateTime SignEffDt { get; set; }

        [JsonPropertyName("signExpDt")]
        public DateTime SignExpDt { get; set; }

        [JsonPropertyName("photoEffDt")]
        public DateTime PhotoEffDt { get; set; }

        [JsonPropertyName("photoExpDt")]
        public DateTime PhotoExpDt { get; set; }
    }
}
