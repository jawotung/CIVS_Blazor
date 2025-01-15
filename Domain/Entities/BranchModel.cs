using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI;

public partial class BranchModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("branchCode")]

    [Display(Name = "Branch Code"),
        StringLength(10, MinimumLength = 3, ErrorMessage = "Branch Code is too short"),
        Required(ErrorMessage = "Branch Code is required")]
    public string BranchCode { get; set; } = null!;
    [JsonPropertyName("branchDesc")]
    [Display(Name = "Branch Desc"),
        StringLength(50, MinimumLength = 10, ErrorMessage = "Branch Desc is too short"),
        Required(ErrorMessage = "Branch Desc is required")]

    public string BranchDesc { get; set; } = null!;
    [JsonPropertyName("branchEmail")]
    [Display(Name = "Branch Email")]
    [EmailAddress]
    [StringLength(50, ErrorMessage = "The {0} must be at most {1} characters long.")]
    public string? BranchEmail { get; set; }
    [JsonPropertyName("isdeleted")]
    [Display(Name = "Deleted")]
    public bool Isdeleted { get; set; }
    [JsonPropertyName("branchBrstn")]
    [Display(Name = "Branch BRSTN"),
        StringLength(10, MinimumLength = 6, ErrorMessage = "Branch BRSTN is too short"),
        Required(ErrorMessage = "Branch BRSTN is required")]
    public string BranchBrstn { get; set; } = null!;
}
