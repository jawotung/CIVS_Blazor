using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebAPI;

public partial class InwardClearingChequeImageModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("chequeImageLinkedKey")]

    public string? ChequeImageLinkedKey { get; set; }
    [JsonPropertyName("chequeImageFileContent")]

    public string? ChequeImageFileContent { get; set; }
    [JsonPropertyName("chequeImageFileContentType")]

    public string? ChequeImageFileContentType { get; set; }
    [JsonPropertyName("chequeImageFileName")]

    public string? ChequeImageFileName { get; set; }
}
