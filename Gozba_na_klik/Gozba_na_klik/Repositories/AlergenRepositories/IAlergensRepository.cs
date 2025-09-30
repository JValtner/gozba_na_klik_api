using Gozba_na_klik.Models.MealModels;

namespace Gozba_na_klik.Repositories.AlergenRepositories
{
    public interface IAlergensRepository
    {
        Task<IEnumerable<Alergen>> GetAllAsync();
        Task<Alergen?> GetByIdAsync(int alergenId);
        Task<Alergen> AddAsync(Alergen alergen);
        Task<Alergen> UpdateAsync(Alergen alergen);
        Task<bool> DeleteAsync(int alergenId);
        Task<bool> ExistsAsync(int alergenId);
    }
}
