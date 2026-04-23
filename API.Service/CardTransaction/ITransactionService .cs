using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using model= API.Model;

namespace API.Service.CardTransaction
{
    public interface ITransactionService
    {
        Task CreateAsync(model.CardTransaction cardTransaction);
    }
}
