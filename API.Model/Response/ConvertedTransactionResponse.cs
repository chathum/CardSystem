using System;
using System.Collections.Generic;
using System.Text;

namespace API.Model.Response
{
    public class ConvertedTransactionResponse
    {
        public Guid TransactionId { get; set; }
        public string OriginalCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal? ConvertedAmount { get; set; }
        public decimal? ExchangeRate { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime RateDateUsed { get; set; }
        public string Description { get; set; } 
    }
}
