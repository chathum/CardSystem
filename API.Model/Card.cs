using API.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Model
{
    public class Card: BaseModel
    {        
        public decimal CreditLimit { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
