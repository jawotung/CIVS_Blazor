using System;
using System.Collections.Generic;

namespace WebAPI;

public partial class UserAuthenticationModel
{
    public long Id { get; set; }

    public string? AuthType { get; set; }

    public string UserId { get; set; } = null!;

    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public DateTime AccessTokenExpiry { get; set; }

    public DateTime RefreshTokenExpiry { get; set; }

    public bool? IsValid { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
