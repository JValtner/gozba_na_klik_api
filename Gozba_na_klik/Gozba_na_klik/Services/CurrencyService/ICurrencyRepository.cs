namespace Gozba_na_klik.Services.CurrencyService
{
    public interface ICurrencyRepository
    {
        Task<decimal> GetRateAsync(string from, string to);
    }
}