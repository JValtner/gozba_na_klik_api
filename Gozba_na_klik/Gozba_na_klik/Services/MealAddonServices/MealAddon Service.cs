using Gozba_na_klik.Models.MealModels;
using static Gozba_na_klik.Repositories.MealAddonsRepositories.IMealAddonsRepositories;

namespace Gozba_na_klik.Services.MealAddonServices
{
    public class MealAddonService
    {
        private readonly IMealAddonsRepository _mealAddonsRepository;

        public MealAddonService(IMealAddonsRepository mealAddonsRepository)
        {
            _mealAddonsRepository = mealAddonsRepository;
        }

        public async Task<IEnumerable<MealAddon>> GetAllMealAddonsAsync()
        {
            return await _mealAddonsRepository.GetAllAsync();
        }

        public async Task<MealAddon?> GetMealAddonByIdAsync(int mealId)
        {
            return await _mealAddonsRepository.GetByIdAsync(mealId);
        }

        public async Task<MealAddon> CreateMealAddonAsync(MealAddon mealAddon)
        {
            return await _mealAddonsRepository.AddAsync(mealAddon);
        }

        public async Task<MealAddon> UpdateMealAddonAsync(MealAddon mealAddon)
        {
            return await _mealAddonsRepository.UpdateAsync(mealAddon);
        }

        public async Task DeleteMealAddonAsync(int mealAddonId)
        {
            await _mealAddonsRepository.DeleteAsync(mealAddonId);
        }



        public async Task<bool> MealAddonExistsAsync(int mealAddonId)
        {
            return await _mealAddonsRepository.ExistsAsync(mealAddonId);
        }
    }
}
