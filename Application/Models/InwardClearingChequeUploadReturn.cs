using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebAPI;

namespace Application.Models
{
    public class InwardClearingChequeUploadReturn
    {
        [JsonPropertyName("detailsModelList")]
        public List<InwardClearingChequeDetailsModel> DetailsModelList { get; set; } = new();
        [JsonPropertyName("imageModelList")]
        public List<InwardClearingChequeImageModel> ImageModelList { get; set; } = new();
        [JsonPropertyName("historyModelList")]
        public List<InwardClearingChequeHistoryModel> HistoryModelList { get; set; } = new();
        [JsonPropertyName("accountModelList")]
        public List<ChequeAccountDetail> AccountModelList { get; set; } = new();
    }
}
