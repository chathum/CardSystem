using System;
using System.Collections.Generic;
using System.Text;

namespace API.Model
{
    public class ExchangeRate
    {
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public decimal Rate { get; set; }
    }
}
