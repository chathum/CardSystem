using System;
using System.Collections.Generic;
using System.Text;

namespace API.Model.Response
{
    public class Treasury
    {
        public string currency { get; set; }
        public DateTime record_date { get; set; }
        public decimal exchange_rate { get; set; }
    }
}
