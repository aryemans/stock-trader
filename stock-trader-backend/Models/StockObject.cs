using System;
namespace stock_trader_backend.Models
{
	public struct StockObject
	{
        public string? companyName { get; set; }
        public string? stock_ticker { get; set; }
        public int? volume { get; set; }
        public string? opening_price { get; set; }
        public string? current_price { get; set; }
        public string? high { get; set; }
        public string? low { get; set; }
        public double? market_capitalization { get; set; }
        public DateTime? expire_date { get; set; }
    }
}

