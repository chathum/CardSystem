using System;
using System.Collections.Generic;
using System.Text;

namespace API.Model.Response
{
    public class BalanceResponse
    {
        public Guid CardId { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal? AvailableBalance { get; set; }
        public string Currency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? ConvertedBalance { get; set; }
        public DateTime RateDateUsed { get; set; }
    }
}
