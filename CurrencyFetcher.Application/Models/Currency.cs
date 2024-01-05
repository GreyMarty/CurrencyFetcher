using System;

namespace CurrencyFetcher.Application.Models
{
    public class Currency
    {
        public DateTime Date { get; set; }
        public string Abbreviation { get; set; }
        public CurrencyName Name { get; set; }
        public decimal? OfficialRate { get; set; }
    }
}