using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebAPI;

public partial class InwardClearingChequeHistoryModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("chequeImageLinkedKey")]

    public string? ChequeImageLinkedKey { get; set; }
    [JsonPropertyName("checkStatusTo")]

    public string? CheckStatusTo { get; set; }
    [JsonPropertyName("reason")]

    public string? Reason { get; set; }
    [JsonPropertyName("branchCode")]

    public string? BranchCode { get; set; }
    [JsonPropertyName("clearingOfficer")]

    public string? ClearingOfficer { get; set; }
    [JsonPropertyName("actionBy")]

    public string? ActionBy { get; set; }
    [JsonPropertyName("actionDateTime")]

    public DateTime ActionDateTime { get; set; }
    [JsonPropertyName("checkStatusFrom")]

    public string? CheckStatusFrom { get; set; }
}
