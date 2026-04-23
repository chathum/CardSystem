using System;
using System.Collections.Generic;
using System.Text;

namespace API.Utils
{
    public interface ITreasuryClient
    {
        Task<decimal?> GetBestRateAsync(string currency, DateTime transactionDate);
    }
}
