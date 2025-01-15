using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI;

public partial class RejectReasonModel
{
    public int Id { get; set; }

    [Display(Name = "Reject Reason Code")]
    public string RejectReasonCode { get; set; } = "";

    [Display(Name = "Reject Reason Desc"),
        StringLength(100, MinimumLength = 10, ErrorMessage = "Reject Reason  Desc is too short"),
        Required(ErrorMessage = "Reject Reason  Desc is required")]
    public string RejectReasonDesc { get; set; }

    [Display(Name = "Deleted")]
    public bool Isdeleted { get; set; }
}
