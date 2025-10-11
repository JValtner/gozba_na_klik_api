using AutoMapper;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;

namespace Gozba_na_klik.Services
{
    public class MealAddonService : IMealAddonService
    {
        private readonly IMealAddonsRepository _mealAddonsRepository;
        private readonly IMapper _mapper;

        public MealAddonService(IMealAddonsRepository mealAddonsRepository, IMapper mapper)
        {
            _mealAddonsRepository = mealAddonsRepository;
            _mapper = mapper;
        }

        // ---------- GET ALL ----------
        public async Task<IEnumerable<ResponseAddonDTO>> GetAllMealAddonsAsync()
        {
            var addons = await _mealAddonsRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ResponseAddonDTO>>(addons);
        }

        // ---------- GET BY ID ----------
        public async Task<ResponseAddonDTO> GetMealAddonByIdAsync(int id)
        {
            var addon = await _mealAddonsRepository.GetByIdAsync(id);
            if (addon == null)
                throw new NotFoundException($"Meal addon with ID {id} not found.");

            return _mapper.Map<ResponseAddonDTO>(addon);
        }

        // ---------- CREATE ----------
        public async Task<ResponseAddonDTO> CreateMealAddonAsync(RequestAddonDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new BadRequestException("Addon name is required.");

            var entity = _mapper.Map<MealAddon>(request);
            var created = await _mealAddonsRepository.AddAsync(entity);

            return _mapper.Map<ResponseAddonDTO>(created);
        }

        // ---------- DELETE ----------
        public async Task DeleteMealAddonAsync(int id)
        {
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
