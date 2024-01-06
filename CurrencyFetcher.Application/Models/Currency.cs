using System;
using Newtonsoft.Json;

namespace CurrencyFetcher.Application.Models
{
    public class Currency
    {
        public DateTime Date { get; set; }
        [JsonProperty("Cur_Abbreviation")] public string Abbreviation { get; set; } = default!;
        [JsonProperty("Cur_Name")] public string Name { get; set; } = default!;
        [JsonProperty("Cur_OfficialRate")] public decimal? OfficialRate { get; set; }
    }
}