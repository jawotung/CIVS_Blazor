using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI;

public partial class UserTypeModel
{
    public int Id { get; set; }

    [Display(Name = "User Type Code"),
        StringLength(10, MinimumLength = 3, ErrorMessage = "User Type Code is too short"),
        Required(ErrorMessage = "User Type Code is required")]
    public string UserTypeCode { get; set; }

    [Display(Name = "User Type Desc"),
        StringLength(50, MinimumLength = 10, ErrorMessage = "User Type Desc is too short"),
        Required(ErrorMessage = "User Type Desc is required")]
    public string UserTypeDesc { get; set; }

    [Display(Name = "Deleted")]
    public bool Isdeleted { get; set; }
}
