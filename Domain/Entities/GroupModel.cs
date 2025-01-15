using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebAPI;

public partial class GroupModel
{
    public int Id { get; set; }

    public string GroupCode { get; set; } = null!;

    public string GroupDesc { get; set; } = null!;

    public bool Isdeleted { get; set; }
   
}
