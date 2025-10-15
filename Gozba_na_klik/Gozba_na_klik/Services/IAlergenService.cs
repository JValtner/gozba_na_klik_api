using Gozba_na_klik.DTOs.Response;

namespace Gozba_na_klik.Services
{
    public interface IAlergenService
    {
        Task<IEnumerable<ResponseAlergenDto>> GetAllAlergenAsync();
        Task<IEnumerable<ResponseAlergenDto>> GetAlergenByMealIdAsync(int mealId);
        Task<ResponseAlergenDto> AddAlergenToMealAsync(int mealId, int alergenId);
        Task<ResponseAlergenDto> RemoveAlergenFromMealAsync(int mealId, int alergenId);
    }
}
