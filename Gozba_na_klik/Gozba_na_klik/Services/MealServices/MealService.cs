using Gozba_na_klik.Models.MealModels;
using Gozba_na_klik.Repositories.MealRepositories;

namespace Gozba_na_klik.Services.MealServices
{
    public class MealService : IMealService
    {
        private readonly IMealsRepository _mealsRepository;

        public MealService(IMealsRepository mealsRepository)
        {
            _mealsRepository = mealsRepository;
        }

        public async Task<IEnumerable<Meal>> GetAllMealsAsync()
        {
            return await _mealsRepository.GetAllAsync();
        }

        public async Task<Meal?> GetMealByIdAsync(int mealId)
        {
            return await _mealsRepository.GetByIdAsync(mealId);
        }

        public async Task<Meal> CreateMealAsync(Meal meal)
        {
            return await _mealsRepository.AddAsync(meal);
        }

        public async Task<Meal> UpdateMealAsync(Meal meal)
        {
            return await _mealsRepository.UpdateAsync(meal);
        }

        public async Task DeleteMealAsync(int mealId)
        {
            await _mealsRepository.DeleteAsync(mealId);
        }



        public async Task<bool> MealExistsAsync(int mealId)
        {
            return await _mealsRepository.ExistsAsync(mealId);
        }
    }
}
