using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI;

public partial class EmailTemplateModel
{
    public int Id { get; set; }
    [Display(Name = "Email For"),
        Required(ErrorMessage = "Email For is required")]

    public string? EmailFor { get; set; }
    [Display(Name = "Email Subject"),
        Required(ErrorMessage = "Email Subject is required")]

    public string? EmailSubjest { get; set; }
    [Display(Name = "Email Body"),
        Required(ErrorMessage = "Email Body is required")]

    public string? EmailBody { get; set; }
}
