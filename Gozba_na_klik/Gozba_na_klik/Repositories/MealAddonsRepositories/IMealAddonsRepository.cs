using Gozba_na_klik.Models.MealModels;

namespace Gozba_na_klik.Repositories.MealAddonsRepositories
{
    public class IMealAddonsRepository
    {
        public interface IMealAddonsRepository
        {
            Task<IEnumerable<MealAddon>> GetAllAsync();
            Task<MealAddon?> GetByIdAsync(int mealAddonId);
            Task<MealAddon> AddAsync(MealAddon mealAddon);
            Task<MealAddon> UpdateAsync(MealAddon mealAddon);
            Task<bool> DeleteAsync(int mealAddonId);
            Task<bool> ExistsAsync(int mealAddonId);
        }
    }
}
