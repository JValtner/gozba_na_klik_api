using Gozba_na_klik.Services.CurrencyService;
using Newtonsoft.Json;

namespace Gozba_na_klik.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _endpoint;
        private readonly string _apiKey;

        public CurrencyRepository(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["CurrencyApi:BaseUrl"];
            _endpoint = configuration["CurrencyApi:Endpoint"];
            _apiKey = configuration["CurrencyApi:ApiKey"];
        }

        public async Task<decimal> GetRateAsync(string from, string to)
        {
            var url = $"{_baseUrl}{_endpoint}?access_key={_apiKey}&base={from}&symbols={to}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            if (result?.rates == null || result.rates[to] == null)
                throw new HttpRequestException("Invalid response from currency API.");

            return (decimal)result.rates[to];
        }
    }
}
