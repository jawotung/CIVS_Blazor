using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI;

public partial class UserModel
{
    public int Id { get; set; }
    [Display(Name = "User ID"),
            StringLength(50, MinimumLength = 3, ErrorMessage = "User name is too short"),
            Required(ErrorMessage = "User name is required")]

    public string UserId { get; set; } = null!;

    public string? UserDisplayName { get; set; }
    [Display(Name = "User Type"),
        StringLength(10),
        Required(ErrorMessage = "User Type is required")]

    public string? UserType { get; set; }
    [Display(Name = "Branch of Assignment"),
        Required(ErrorMessage = "Branch of Assignment is required")]

    public string? BranchOfAssignment { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime LastLoginDate { get; set; }

    public bool? Isdisabled { get; set; }
    [Display(Name = "EmployeeNumber"),
        StringLength(10),
        RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "AlphaNumeric Only")]

    public string? EmployeeNumber { get; set; }

    public string? LastLoginSession { get; set; }
    [NotMapped]
    [Display(Name = "User Type")]
    public string? UserTypeDesc { get; set; } = "";

    [NotMapped]
    public string? GroupCode { get; set; } = "";

    [NotMapped]
    [Display(Name = "Grouping")]
    public string? GroupingDesc { get; set; } = "";

    [NotMapped]
    [Display(Name = "Branch of Assignment")]
    public string? BranchOfAssignmentDesc { get; set; } = "";
}
