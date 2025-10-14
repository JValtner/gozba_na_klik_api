namespace Gozba_na_klik.Models
{
    public interface IMealsRepository
    {
        Task<IEnumerable<Meal>> GetMealsByRestaurantIdAsync(int restaurantId);
        Task<Meal?> GetByIdAsync(int userId);
        Task<Meal> AddAsync(Meal meal);
        Task<Meal> UpdateAsync(Meal meal);
        Task<bool> DeleteAsync(int mealId);
        Task<bool> ExistsAsync(int mealId);
    }
}
