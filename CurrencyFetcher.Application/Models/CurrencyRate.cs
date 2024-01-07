using System;
using System.Text.Json.Serialization;

namespace CurrencyFetcher.Application.Models
{
    public class CurrencyRate
    {
        public DateTime Date { get; set; }
        [JsonPropertyName("Cur_Abbreviation")] public string Abbreviation { get; set; } = default!;
        [JsonPropertyName("Cur_Name")] public string Name { get; set; } = default!;
        [JsonPropertyName("Cur_OfficialRate")] public decimal? OfficialRate { get; set; }
        [JsonPropertyName("Cur_Scale")] public decimal Scale { get; set; }
    }
}