using System;
using Newtonsoft.Json;

namespace CurrencyFetcher.Application.Models
{
    internal class JsonCurrency
    {
        public DateTime Date { get; set; }
        
        [JsonProperty("Cur_Abbreviation")]
        public string Abbreviation { get; set; }
        
        [JsonProperty("Cur_Name")]
        public string Name { get; set; }
        
        [JsonProperty("Cur_OfficialRate")]
        public decimal? OfficialRate { get; set; }
    }
}