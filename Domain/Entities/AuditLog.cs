using System;
using System.Collections.Generic;

namespace WebAPI;

public partial class AuditLog
{
    public int Id { get; set; }

    public string? Log { get; set; }

    public DateTime LogTime { get; set; }
}
