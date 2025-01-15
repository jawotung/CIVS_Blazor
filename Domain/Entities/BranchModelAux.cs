using System;
using System.Collections.Generic;

namespace WebAPI;

public partial class BranchModelAux
{
    public int Id { get; set; }

    public string BranchCode { get; set; } = null!;

    public string BranchBuddyCode { get; set; } = null!;

    public bool BranchBcp { get; set; }
}
