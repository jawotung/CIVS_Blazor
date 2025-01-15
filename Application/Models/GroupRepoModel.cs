using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ReturnDownloadPDF
    {
        [JsonPropertyName("pdfDataBase64")]
        public string PdfDataBase64 { get; set; }
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
    }

    public class ParamGroupModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("groupCode")]
        public string GroupCode { get; set; } = null!;
        [JsonPropertyName("groupDesc")]
        public string GroupDesc { get; set; } = null!;
        [JsonPropertyName("isdeleted")]
        public bool Isdeleted { get; set; }

    }

    public class ReturnGroup
    {
        public int Id { get; set; }

        public string GroupCode { get; set; } = null!;

        public string GroupDesc { get; set; } = null!;

        public bool Isdeleted { get; set; }
        public string AccessAction { get; set; }
        public string MemberAction { get; set; }

    }

}
