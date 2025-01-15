using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ParamLoginCredentials
    {
        [JsonPropertyName("userid")]
        [Display(Name = "User ID"),
        StringLength(50, MinimumLength = 3, ErrorMessage = "User name is too short"),
        Required(ErrorMessage = "User name is required")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        [Display(Name = "Password"),
        DataType(DataType.Password),
        Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        public string? IpAddress { get; set; } = string.Empty;
        public string? UserAgent { get; set; } = string.Empty;

    }

    public enum EnumValidateCredDetails
    {
        isValid,
        DisplayName,
        ErrorMessage
    }

    public class ReturnLoginCredentials
    {
        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; } = string.Empty;
        [JsonPropertyName("statusMessage")]
        public string StatusMessage { get; set; } = string.Empty;
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = string.Empty;
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;
        [JsonPropertyName("accessTokenExpiresIn")]
        public string AccessTokenExpiresIn { get; set; } = string.Empty;
        [JsonPropertyName("refreshTokenExpiresIn")]
        public string RefreshTokenExpiresIn { get; set; } = string.Empty;

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; } = string.Empty;
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        public DateTime LastLoginDate { get; set; }
        [JsonPropertyName("displayName")]

        public string DisplayName { get; set; } = string.Empty;
        [JsonPropertyName("userTypeDesc")]

        public string UserTypeDesc { get; set; } = string.Empty;
        [JsonPropertyName("groupingDesc")]

        public string GroupingDesc { get; set; } = string.Empty;
        [JsonPropertyName("branchOfAssignmentCode")]

        public string BranchOfAssignmentCode { get; set; } = string.Empty;
        [JsonPropertyName("branchOfAssignmentDesc")]

        public string BranchOfAssignmentDesc { get; set; } = string.Empty;
        [JsonPropertyName("menuDesc")]

        public string MenuDesc { get; set; } = string.Empty;
        [JsonPropertyName("bcpBranch")]

        public BCPBranch BCPBranch { get; set; }
    }

    public class BCPBranch
    {
        public string BranchBuddyCode { get; set; }
        public string BranchBuddyDesc { get; set; }
    }

    public class MUserModel
    {
        public int ID { get; set; }

        [Display(Name = "User ID"),
            StringLength(50, MinimumLength = 3, ErrorMessage = "User name is too short"),
            Required(ErrorMessage = "User name is required")]
        public string UserID { get; set; }

        [Display(Name = "User Name")]
        public string UserDisplayName { get; set; }

        [Display(Name = "User Type"),
            StringLength(10),
            Required(ErrorMessage = "User Type is required")]
        public string UserType { get; set; }

        [Display(Name = "Branch of Assignment"),
            Required(ErrorMessage = "Branch of Assignment is required")]
        public string BranchOfAssignment { get; set; }

        [Display(Name = "Deactivated")]
        public bool ISDeleted { get; set; }

        [Display(Name = "Disabled")]
        public bool ISDisabled { get; set; }

        [Display(Name = "EmployeeNumber"),
            StringLength(10),
            RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "AlphaNumeric Only")]
        public string EmployeeNumber { get; set; }

        [Display(Name = "Last Login Date")]
        public DateTime LastLoginDate { get; set; }

        public string LastLoginSession { get; set; }

        [NotMapped]
        [Display(Name = "User Type")]
        public string UserTypeDesc { get; set; }

        [NotMapped]
        public string GroupCode { get; set; }

        [NotMapped]
        [Display(Name = "Grouping")]
        public string GroupingDesc { get; set; }

        [NotMapped]
        [Display(Name = "Branch of Assignment")]
        public string BranchOfAssignmentDesc { get; set; }
    }

    public class ParamSaveApiAuthentication
    {
        public string Type { get; set; }
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }

}
