using Gozba_na_klik.Models.MealModels;
using Gozba_na_klik.Repositories.AlergenRepositories;
using static Gozba_na_klik.Repositories.AlergenRepositories.IAlergensRepository;

namespace Gozba_na_klik.Services.AlergenServices
{
    public class AlergenService: IAlergenService
    {
        private readonly IAlergensRepository _alergensRepository;

        public AlergenService(IAlergensRepository alergensRepository)
        {
            _alergensRepository = alergensRepository;
        }

        public async Task<IEnumerable<Alergen>> GetAllAlergenAsync()
        {
            return await _alergensRepository.GetAllAsync();
        }

        public async Task<Alergen?> GetAlergenByIdAsync(int alergenId)
        {
            return await _alergensRepository.GetByIdAsync(alergenId);
        }

        public async Task<Alergen> CreateAlergenAsync(Alergen alergen)
        {
            return await _alergensRepository.AddAsync(alergen);
        }

        public async Task<Alergen> UpdateAlergenAsync(Alergen alergen)
        {
            return await _alergensRepository.UpdateAsync(alergen);
        }

        public async Task DeleteAlergenAsync(int alergenId)
        {
            await _alergensRepository.DeleteAsync(alergenId);
        }



        public async Task<bool> AlergenExistsAsync(int alergenId)
        {
            return await _alergensRepository.ExistsAsync(alergenId);
        }
    }
}
