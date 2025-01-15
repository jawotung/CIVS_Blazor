using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebAPI;

public partial class ChequeAccountDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("chequeImageLinkedKey")]

    public string? ChequeImageLinkedKey { get; set; }
    [JsonPropertyName("accountNumber")]

    public string? AccountNumber { get; set; }
    [JsonPropertyName("accountName")]

    public string? AccountName { get; set; }
    [JsonPropertyName("accountStatus")]

    public string? AccountStatus { get; set; }
    [JsonPropertyName("effectivityDate")]

    public DateTime? EffectivityDate { get; set; }
    [JsonPropertyName("statusAsOfDate")]

    public DateTime? StatusAsOfDate { get; set; }
}
