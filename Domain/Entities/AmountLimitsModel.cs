using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI;

public partial class AmountLimitsModel
{

    public int Id { get; set; }

    [Display(Name = "Amount Limit Code"),
        Required(ErrorMessage = "Required Field"),
        StringLength(10)]
    public string AmountLimitsCode { get; set; }

    [Display(Name = "Amount Limit Desc"),
        Required(ErrorMessage = "Required Field"),
        StringLength(200)]
    public string AmountLimitsDesc { get; set; }

    [Display(Name = "Max Amount Limit"), DisplayFormat(DataFormatString = "{0:#,##0.00}"),
        Required(ErrorMessage = "Required Field")]
    public double MaxAmountLimit { get; set; }

    [Display(Name = "Allowed Action"),
        StringLength(20)]
    public string AllowedAction { get; set; } = "";

    [Display(Name = "Deleted")]
    public bool? Isdeleted { get; set; }

    [NotMapped]
    public IList<string> SelectedActions { get; set; }
}
