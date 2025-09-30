using Gozba_na_klik.Models.MealModels;

namespace Gozba_na_klik.Services.MealServices
{
    public interface IMealService
    {
        Task<IEnumerable<Meal>> GetAllMealsAsync();
        Task<Meal?> GetMealByIdAsync(int mealId);
        Task<bool> MealExistsAsync(int mealId);
        Task<Meal> CreateMealAsync(Meal meal);
        Task<Meal> UpdateMealAsync(Meal meal);
        Task DeleteMealAsync(int mealId);
    }
}
