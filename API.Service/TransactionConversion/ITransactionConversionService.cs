using API.Model.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Service.TransactionConversion
{
    public interface ITransactionConversionService
    {
        Task<ConvertedTransactionResponse> ConvertTransactionAsync(Guid transactionId, string targetCurrency);
        Task<BalanceResponse> GetAvailableBalanceAsync(Guid cardId, string targetCurrency);
    }
}
