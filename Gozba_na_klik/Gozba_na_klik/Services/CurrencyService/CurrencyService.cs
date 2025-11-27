using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Repositories;

namespace Gozba_na_klik.Services.CurrencyService
{
    public class CurrencyService:ICurrencyService
    {
        private static readonly Dictionary<string, decimal> _cache = new();
        private readonly ICurrencyRepository _currencyRepository;

        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public async Task<decimal> ConvertAsync(decimal amount, string from, string to)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                throw new BadRequestException("Currency 'from' and 'to' must be provided.");

            if (amount < 0)
                throw new BadRequestException("Amount must be non-negative.");

            string cacheKey = $"{from}_{to}";
            decimal rate;

            if (_cache.ContainsKey(cacheKey))
            {
                rate = _cache[cacheKey];
            }
            else
            {
                rate = await _currencyRepository.GetRateAsync(from, to);
                _cache[cacheKey] = rate;
            }

            return amount * rate;
        }
    }
}
