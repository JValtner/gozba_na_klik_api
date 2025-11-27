
namespace Gozba_na_klik.Services.CurrencyService
{
    public interface ICurrencyService
    {
        Task<decimal> ConvertAsync(decimal amount, string from, string to);
    }
}