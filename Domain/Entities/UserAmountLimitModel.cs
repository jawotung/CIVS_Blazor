using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI;

public partial class UserAmountLimitModel
{
    public int Id { get; set; }

    [Display(Name = "User ID"),
        Required(ErrorMessage = "Required Field")]
    public string UserId { get; set; }

    [Display(Name = "Amount Limit Code"),
        Required(ErrorMessage = "Required Field")]
    public string AmountLimitId { get; set; }

    [Display(Name = "Deleted")]
    public bool? Isdeleted { get; set; }

    [NotMapped]
    [Display(Name = "User Type")]
    public string UserType { get; set; } = "";
    [NotMapped]
    [Display(Name = "User ID")]
    public string UserDisplay { get; set; } = "";
    [NotMapped]
    [Display(Name = "Amount Limit")]
    public string AmountLimitDesc { get; set; } = "";
    [NotMapped]
    [Display(Name = "Amount Limit")]
    public string AmountLimitDescMore { get; set; } = "";
    [NotMapped]
    [Display(Name = "Limits Desc")]
    public string LimitsDesc { get; set; } = "";
    [NotMapped]
    [Display(Name = "Limit Amount")]
    public string LimitAmount { get; set; } = "";
    [NotMapped]
    [Display(Name = "Limit Action")]
    public string LimitAction { get; set; } = "";

}
