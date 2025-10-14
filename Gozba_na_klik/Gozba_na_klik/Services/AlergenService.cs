using AutoMapper;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;
using Microsoft.Extensions.Logging;

namespace Gozba_na_klik.Services
{
    public class AlergenService : IAlergenService
    {
        private readonly IAlergensRepository _alergensRepository;
        private readonly IMealsRepository _mealsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AlergenService> _logger;

        public AlergenService(
            IAlergensRepository alergensRepository,
            IMealsRepository mealsRepository,
            IMapper mapper,
            ILogger<AlergenService> logger)
        {
            _alergensRepository = alergensRepository;
            _mealsRepository = mealsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ResponseAlergenDto>> GetAllAlergenAsync()
        {
            _logger.LogInformation("Fetching all allergens from repository...");
            var alergens = await _alergensRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ResponseAlergenDto>>(alergens);
        }

        public async Task<IEnumerable<ResponseAlergenDto>> GetAlergenByMealIdAsync(int mealId)
        {
            _logger.LogInformation("Fetching allergens for meal {MealId}", mealId);
            var alergens = await _alergensRepository.GetAlergenByMealIdAsync(mealId);
            return _mapper.Map<IEnumerable<ResponseAlergenDto>>(alergens);
        }

        public async Task<ResponseAlergenDto> AddAlergenToMealAsync(int mealId, int alergenId)
        {
            _logger.LogInformation("Adding allergen {AlergenId} to meal {MealId}", alergenId, mealId);

            var alergen = await _alergensRepository.AddAlergenToMealAsync(mealId, alergenId);
            if (alergen == null)
                throw new NotFoundException($"Failed to add allergen {alergenId} to meal {mealId}.");

            _logger.LogInformation("Successfully added allergen {AlergenId} to meal {MealId}", alergenId, mealId);
            return _mapper.Map<ResponseAlergenDto>(alergen);
        }

        public async Task<ResponseAlergenDto> RemoveAlergenFromMealAsync(int mealId, int alergenId)
        {
            _logger.LogInformation("Removing allergen {AlergenId} from meal {MealId}", alergenId, mealId);

            var alergen = await _alergensRepository.RemoveAlergenFromMealAsync(mealId, alergenId);
            if (alergen == null)
                throw new NotFoundException($"Failed to remove allergen {alergenId} from meal {mealId}.");

            _logger.LogInformation("Successfully removed allergen {AlergenId} from meal {MealId}", alergenId, mealId);
            return _mapper.Map<ResponseAlergenDto>(alergen);
        }
    }
}
