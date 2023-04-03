namespace API_TIN_KBI.Models
{
    public class RawTradeFeed
    {
        public Decimal TradeFeedID { get; set; }
        public string? ExchangeRef { get; set; }
        public DateTime BusinessDate { get; set; }
        public DateTime TradeTime { get; set; }
        public string SellerInvCode { get; set; }
        public Decimal Qty { get; set; }
        public Decimal Price { get; set; }
        public string BuyerInvCode { get; set; }
        public string SellerRef { get; set; }
    }
}
