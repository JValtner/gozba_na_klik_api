using Gozba_na_klik.Models.MealModels;

namespace Gozba_na_klik.Repositories.MealRepositories
{
    public interface IMealsRepository
    {
        Task<IEnumerable<Meal>> GetAllAsync();
        Task<Meal?> GetByIdAsync(int userId);
        Task<Meal> AddAsync(Meal meal);
        Task<Meal> UpdateAsync(Meal meal);
        Task<bool> DeleteAsync(int mealId);
        Task<bool> ExistsAsync(int mealId);
    }
}
