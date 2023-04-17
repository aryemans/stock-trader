using System;
namespace stock_trader_backend.Models
{
	public struct LimitOrderObject
    {
        public string? order_id { get; set; }
        public string? stock_ticker { get; set; }
        public string? order_type { get; set; }
        public string? price { get; set; }
        public int? quantity { get; set; }
        public DateTime? expireDate { get; set; }
    }
}

