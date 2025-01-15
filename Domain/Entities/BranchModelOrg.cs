using System;
using System.Collections.Generic;

namespace WebAPI;

public partial class BranchModelOrg
{
    public int Id { get; set; }

    public string BranchCode { get; set; } = null!;

    public string BranchDesc { get; set; } = null!;

    public string? BranchEmail { get; set; }

    public bool Isdeleted { get; set; }

    public string BranchBrstn { get; set; } = null!;
}
