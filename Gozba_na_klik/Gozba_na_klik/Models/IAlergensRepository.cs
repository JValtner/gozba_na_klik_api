using Gozba_na_klik.Models;

namespace Gozba_na_klik.Repositories
{
    public interface IAlergensRepository
    {
        Task<IEnumerable<Alergen>> GetAllAsync();
        Task<Alergen?> GetByIdAsync(int alergenId);
        Task<IEnumerable<Alergen>> GetAlergenByMealIdAsync(int mealId);
        Task<Alergen?> AddAlergenToMealAsync(int mealId, int alergenId);
        Task<Alergen?> RemoveAlergenFromMealAsync(int mealId, int alergenId);
        Task<bool> ExistsAsync(int alergenId);
    }
}
