using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebAPI;

public partial class GroupMemberModel
{
    public int Id { get; set; }

    public string? GroupId { get; set; }

    public string? UserTypes { get; set; }

    public bool Isdeleted { get; set; }
    [NotMapped]
    public IList<string> SelectedMemberIDs { get; set; }
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
    [Display(Name = "Members")]
    public string UserTypesDesc { get; set; }
    //{
    //    get
    //    {
    //        return new CommonClass().GetUserTypeIDsDesc(UserTypes);

    //    }
    //}
}
