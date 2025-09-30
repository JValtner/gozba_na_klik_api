using Gozba_na_klik.Models.MealModels;

namespace Gozba_na_klik.Services.AlergenServices
{
    public interface IAlergenService
    {
        Task<IEnumerable<Alergen>> GetAllAlergenAsync();
        Task<Alergen?> GetAlergenByIdAsync(int alergenId);
        Task<bool> AlergenExistsAsync(int alergenId);
        Task<Alergen> CreateAlergenAsync(Alergen alergen);
        Task<Alergen> UpdateAlergenAsync(Alergen alergen);
        Task DeleteAlergenAsync(int id);
    }
}
