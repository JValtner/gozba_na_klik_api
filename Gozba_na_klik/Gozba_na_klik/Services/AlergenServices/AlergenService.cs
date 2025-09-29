using Gozba_na_klik.Models.MealModels;
using Gozba_na_klik.Repositories.AlergenRepositories;
using static Gozba_na_klik.Repositories.AlergenRepositories.IAlergensRepository;

namespace Gozba_na_klik.Services.AlergenServices
{
    public class AlergenService
    {
        private readonly IAlergensRepository _alergensRepository;

        public AlergenService(IAlergensRepository alergensRepository)
        {
            _alergensRepository = alergensRepository;
        }

        public async Task<IEnumerable<Alergen>> GetAllAlergensAsync()
        {
            return await _alergensRepository.GetAllAsync();
        }

        public async Task<Alergen?> GetMealAddonByIdAsync(int alergenId)
        {
            return await _alergensRepository.GetByIdAsync(alergenId);
        }

        public async Task<Alergen> CreateMealAddonAsync(Alergen alergen)
        {
            return await _alergensRepository.AddAsync(alergen);
        }

        public async Task<Alergen> UpdateMealAddonAsync(Alergen alergen)
        {
            return await _alergensRepository.UpdateAsync(alergen);
        }

        public async Task DeleteMealAddonAsync(int alergenId)
        {
            await _alergensRepository.DeleteAsync(alergenId);
        }



        public async Task<bool> MealAddonExistsAsync(int alergenId)
        {
            return await _alergensRepository.ExistsAsync(alergenId);
        }
    }
}
