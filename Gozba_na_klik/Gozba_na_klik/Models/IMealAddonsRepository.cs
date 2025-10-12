namespace Gozba_na_klik.Models
{
    public interface IMealAddonsRepository
    {
        Task<IEnumerable<MealAddon>> GetAllByMealIdAsync(int mealId);
        Task<MealAddon?> GetByIdAsync(int mealAddonId);
        Task<MealAddon> AddAsync(MealAddon mealAddon);
        Task<bool> SetActiveChosenAddon(int addonId);
        Task<bool> DeleteAsync(int mealAddonId);
        Task<bool> ExistsAsync(int mealAddonId);
    }
}
