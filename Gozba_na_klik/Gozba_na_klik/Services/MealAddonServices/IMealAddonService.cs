using Gozba_na_klik.Models.MealModels;

namespace Gozba_na_klik.Services.MealAddonServices
{
    public interface IMealAddonService
    {
        Task<IEnumerable<MealAddon>> GetAllMealAddonsAsync();
        Task<MealAddon?> GetMealAddonByIdAsync(int mealAddonId);
        Task<bool> MealAddonExistsAsync(int mealAddonId);
        Task<MealAddon> CreateMealAddonAsync(MealAddon mealAddon);
        Task<MealAddon> UpdateMealAddonAsync(MealAddon mealAddon);
        Task DeleteMealAddonAsync(int mealAddonId);
    }
}
