using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebAPI;

public partial class GroupAccessModel
{
    public int Id { get; set; }

    public string? GroupId { get; set; }

    public string? MenuIds { get; set; }
    public bool Isdeleted { get; set; }

    [NotMapped]
    public IList<string> SelectedMenuIDs { get; set; }

    [NotMapped]
    [Display(Name = "Group")]
    public string GroupDesc { get; set; }
    //{
    //    get
    //    {
    //        return new CommonClass().GetGroupDesc(GroupID);
    //    }
    //}

    [NotMapped]
    [Display(Name = "Menu Access")]
    public string MenuIDsDesc { get; set; }
    //{
    //    get
    //    {
    //        return new CommonClass().GetMenuIDsDesc(MenuIDs);
    //    }
    //}
}
