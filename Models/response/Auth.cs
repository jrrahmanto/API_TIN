namespace API_TIN_KBI.Models.response
{
    public class Auth
    {
        public class success
        {
            public string ExchangeRef { get; set; }
            public string BusinessDate { get; set; }
            public string TradeTime { get; set; }
            public string SellerName { get; set; }
            public string BuyerName { get; set; }
            public string TotalTonase { get; set; }
            public string TotalTransaction { get; set; }
            public List<detaillogams> detaillogam { get; set; }
            public string message { get; set; }
        }
        public class detaillogams
        {
            public string noBst { get; set; }
            public string port { get; set; }
            public string product { get; set; }
            public string brand { get; set; }
            public string tonase { get; set; }
            public string price { get; set; }
            public string total { get; set; }
        }
        public class error
        {
            public string message { get; set; }
        }
    }
}
