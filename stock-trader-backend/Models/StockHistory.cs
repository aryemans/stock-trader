using System;
namespace stock_trader_backend.Models
{
	public struct StockHistory
	{
            public string? transaction_type { get; set; }
            public string? stock_ticker { get; set; }
            public int? quantity { get; set; }
            public DateTime? datePurchased { get; set; }
            public double? purchasedPrice { get; set; }
    }
}


