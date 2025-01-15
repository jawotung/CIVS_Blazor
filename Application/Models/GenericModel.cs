using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ReturnGenericStatus
    {
        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; } = string.Empty;
        [JsonPropertyName("statusMessage")]
        public string StatusMessage { get; set; } = string.Empty;
    }

    public class ReturnGenericList<T>
    {
        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; } = string.Empty;
        [JsonPropertyName("statusMessage")]
        public string StatusMessage { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public List<T> Data { get; set; }
    }

    public class ReturnGenericData<T>
    {
        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; } = string.Empty;
        [JsonPropertyName("statusMessage")]
        public string StatusMessage { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    public class ReturnGenericDictionary
    {
        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; } = string.Empty;
        [JsonPropertyName("statusMessage")]
        public string StatusMessage { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public Dictionary<string, List<string>> Data { get; set; }
    }
    public class ReturnGenericDropdown
    {
        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; } = string.Empty;
        [JsonPropertyName("statusMessage")]
        public string StatusMessage { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public List<SelectListItem> Data { get; set; }
    }
    public class MReponseObj
    {
        public string Result { get; set; }
        public bool IsExist { get; set; }
        public bool IsValid { get; set; }
    }
    public class ErrorResponse
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("errors")]
        public Dictionary<string, List<string>> Errors { get; set; }
        [JsonPropertyName("traceId")]
        public string TraceId { get; set; }
    }
}
