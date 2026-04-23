using API.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Model
{
    public class CardTransaction: BaseModel
    {
        public Guid CardId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Currency { get; set; }
    }
}
