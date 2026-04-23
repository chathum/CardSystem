using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Runtime;
using System.Text;
using static System.Net.WebRequestMethods;
using model= API.Model.Response;

namespace API.Utils
{
    public class TreasuryClient : ITreasuryClient
    {
        private readonly HttpClient _http;

        public TreasuryClient(HttpClient http)
        {
            _http = http;
        }


        public async Task<decimal?> GetBestRateAsync(string currency, DateTime transactionDate)
        {
            var baseUrl = AppConfig.Get("TreasuryFxApi:BaseUrl");
            var fromDate = transactionDate.AddMonths(-6);
            var url =
                $"{baseUrl}" +
                $"?filter=currency:eq:{currency}" +
                $",record_date:lte:{transactionDate:yyyy-MM-dd}" +
                $",record_date:gte:{fromDate:yyyy-MM-dd}" +
                "&sort=-record_date";

            var response = await _http.GetFromJsonAsync<model.TreasuryResponse>(url);

            return response?.Data?
                .OrderByDescending(x => x.record_date)
                .FirstOrDefault()
                ?.exchange_rate;
        }
    }
}
