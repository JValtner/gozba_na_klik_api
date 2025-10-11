using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;

namespace Gozba_na_klik.Services
{
    public interface IMealService
    {
        Task<IEnumerable<ResponseMealDto>> GetMealsByRestaurantIdAsync(int restaurantId);
        Task<ResponseMealDto?> GetMealByIdAsync(int mealId);
        Task<bool> MealExistsAsync(int mealId);
        Task<ResponseMealDto> CreateMealAsync(Meal meal);
        Task<ResponseMealDto> UpdateMealAsync(Meal meal);
        Task DeleteMealAsync(int mealId);
    }
}
