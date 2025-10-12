using AutoMapper;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Microsoft.Extensions.Logging;

namespace Gozba_na_klik.Services
{
    public class MealAddonService : IMealAddonService
    {
        private readonly IMealAddonsRepository _mealAddonsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MealAddonService> _logger;

        public MealAddonService(
            IMealAddonsRepository mealAddonsRepository,
            IMapper mapper,
            ILogger<MealAddonService> logger)
        {
            _mealAddonsRepository = mealAddonsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // ---------- GET ALL BY MEAL ----------
        public async Task<IEnumerable<ResponseAddonDTO>> GetAddonsByMealIdAsync(int mealId)
        {
            _logger.LogInformation("Fetching all addons for meal {MealId}", mealId);
            var addons = await _mealAddonsRepository.GetAllByMealIdAsync(mealId);
            return _mapper.Map<IEnumerable<ResponseAddonDTO>>(addons);
        }

        // ---------- GET BY ID ----------
        public async Task<ResponseAddonDTO> GetMealAddonByIdAsync(int id)
        {
            _logger.LogInformation("Fetching addon by ID {AddonId}", id);
            var addon = await _mealAddonsRepository.GetByIdAsync(id)
                        ?? throw new NotFoundException($"Meal addon with ID {id} not found.");

            return _mapper.Map<ResponseAddonDTO>(addon);
        }

        // ---------- CREATE ----------
        public async Task<ResponseAddonDTO> CreateMealAddonAsync(RequestAddonDto request)
        {

            _logger.LogInformation("Creating addon: {AddonName}", request.Name);
            var entity = _mapper.Map<MealAddon>(request);
            var created = await _mealAddonsRepository.AddAsync(entity);

            return _mapper.Map<ResponseAddonDTO>(created);
        }

        // ---------- SET ACTIVE CHOSEN ADDON ----------
        public async Task SetActiveChosenAddon(int addonId)
        {
            var addon = await _mealAddonsRepository.GetByIdAsync(addonId)
                        ?? throw new NotFoundException("Addon not found.");

            if (addon.Type != "chosen")
                throw new BadRequestException("Only chosen addons can be activated.");

            _logger.LogInformation("Activating chosen addon {AddonId} for meal {MealId}", addonId, addon.MealId);

            // Deactivate all chosen addons for this meal
            await _mealAddonsRepository.SetActiveChosenAddon(addonId);
        }

        // ---------- DELETE ----------
        public async Task DeleteMealAddonAsync(int id)
        {
            _logger.LogInformation("Deleting addon ID {AddonId}", id);
            var deleted = await _mealAddonsRepository.DeleteAsync(id);
            if (!deleted)
                throw new NotFoundException($"Meal addon with ID {id} not found.");
        }

        // ---------- EXISTS ----------
        public async Task<bool> MealAddonExistsAsync(int id)
        {
            return await _mealAddonsRepository.ExistsAsync(id);
        }

        
    }
}
