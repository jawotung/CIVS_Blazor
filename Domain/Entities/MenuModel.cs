using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI;

[NotMapped]
public partial class MenuModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("menuCode")]
    [Display(Name = "Menu Code"),
        StringLength(10, MinimumLength = 3, ErrorMessage = "Menu Code is too short"),
        Required(ErrorMessage = "Menu Code is required")]
    public string MenuCode { get; set; } = null!;

    [JsonPropertyName("menuDesc")]
    [Display(Name = "Menu Desc"),
        StringLength(100, MinimumLength = 10, ErrorMessage = "Menu Desc is too short"),
        Required(ErrorMessage = "Menu Desc is required")]
    public string MenuDesc { get; set; } = null!;
    [Display(Name = "Contoller")]
    public string? Controller { get; set; }
    [Display(Name = "Action Method")]
    public string? ActionMethod { get; set; }
    [Display(Name = "Action Method Parameters")]
    public string? ActionMethodParam { get; set; }
    [Display(Name = "is Root Menu ?")]
    public bool RootMenu { get; set; }
    [Display(Name = "Sub Menu")]
    public string? SubMenus { get; set; }
    [Display(Name = "Deleted")]
    public bool Isdeleted { get; set; }
    [NotMapped]
    public IList<string> SelectedSubMenus { get; set; }
    [NotMapped]
    [Display(Name = "Sub Menu")]
    public string SubMenusDesc { get; set; } = "";
    //{
    //    get
    //    {
    //        CommonClass _commonClass = new CommonClass();
    //        if (RootMenu == true)
    //        {
    //            var x = _commonClass.GetMenuIDsDesc(SubMenus);
    //            return x;
    //        }
    //        return "";
    //    }
    //}
}
