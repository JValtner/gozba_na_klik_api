using AutoMapper;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Gozba_na_klik.Services
{
    public class MealService : IMealService
    {
        private readonly IMealsRepository _mealsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MealService> _logger;

        private const string DefaultMealImagePath = "/assets/mealImg/default_meal.png";

        public MealService(IMealsRepository mealsRepository, IMapper mapper, ILogger<MealService> logger)
        {
            _mealsRepository = mealsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ResponseMealDto>> GetMealsByRestaurantIdAsync(int restaurantId)
        {
            if (restaurantId <= 0)
                throw new BadRequestException("Invalid restaurant ID.");

            _logger.LogInformation("Fetching meals for restaurant ID: {RestaurantId}", restaurantId);

            var meals = await _mealsRepository.GetMealsByRestaurantIdAsync(restaurantId);

            foreach (var meal in meals)
                if (string.IsNullOrEmpty(meal.ImagePath))
                    meal.ImagePath = DefaultMealImagePath;

            var mealDtos = _mapper.Map<IEnumerable<ResponseMealDto>>(meals);

            _logger.LogInformation("Retrieved {MealCount} meals for restaurant {RestaurantId}", mealDtos.Count(), restaurantId);
            return mealDtos;
        }

        public async Task<ResponseMealDto?> GetMealByIdAsync(int mealId)
        {
            if (mealId <= 0)
                throw new BadRequestException("Invalid meal ID.");

            _logger.LogInformation("Fetching meal with ID: {MealId}", mealId);

            var meal = await _mealsRepository.GetByIdAsync(mealId);
            if (meal == null)
            {
                _logger.LogWarning("Meal with ID {MealId} not found.", mealId);
                throw new NotFoundException($"Meal with ID {mealId} not found.");
            }

            if (string.IsNullOrEmpty(meal.ImagePath))
                meal.ImagePath = DefaultMealImagePath;

            return _mapper.Map<ResponseMealDto>(meal);
        }

        public async Task<ResponseMealDto> CreateMealAsync(Meal meal)
        {
            if (meal == null)
                throw new BadRequestException("Meal cannot be null.");

            _logger.LogInformation("Creating meal: {MealData}", JsonSerializer.Serialize(meal));

            if (string.IsNullOrWhiteSpace(meal.Name))
                throw new BadRequestException("Meal name is required.");
            if (meal.RestaurantId <= 0)
                throw new BadRequestException("Invalid restaurant ID.");

            var created = await _mealsRepository.AddAsync(meal);

            _logger.LogInformation("Meal created with ID {MealId}", created.Id);
            return _mapper.Map<ResponseMealDto>(created);
        }

        public async Task<ResponseMealDto> UpdateMealAsync(Meal meal)
        {
            if (meal == null)
                throw new BadRequestException("Meal cannot be null.");

            _logger.LogInformation("Updating meal with ID {MealId}", meal.Id);

            if (!await _mealsRepository.ExistsAsync(meal.Id))
            {
                _logger.LogWarning("Meal with ID {MealId} not found for update.", meal.Id);
                throw new NotFoundException($"Meal with ID {meal.Id} not found.");
            }

            var updated = await _mealsRepository.UpdateAsync(meal);
            _logger.LogInformation("Meal updated successfully: {MealId}", updated.Id);

            return _mapper.Map<ResponseMealDto>(updated);
        }

        public async Task DeleteMealAsync(int mealId)
        {
            if (mealId <= 0)
                throw new BadRequestException("Invalid meal ID.");

            _logger.LogInformation("Deleting meal with ID {MealId}", mealId);

            var meal = await _mealsRepository.GetByIdAsync(mealId);
            if (meal == null)
            {
                _logger.LogWarning("Meal with ID {MealId} not found for deletion.", mealId);
                throw new NotFoundException($"Meal with ID {mealId} not found.");
            }

            await _mealsRepository.DeleteAsync(mealId);
            _logger.LogInformation("Meal with ID {MealId} deleted successfully.", mealId);
        }

        public async Task<bool> MealExistsAsync(int mealId)
        {
            if (mealId <= 0)
                throw new BadRequestException("Invalid meal ID.");

            var exists = await _mealsRepository.ExistsAsync(mealId);
            _logger.LogDebug("Meal exists check for ID {MealId}: {Exists}", mealId, exists);
            return exists;
        }
    }
}
