using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;

namespace Gozba_na_klik.Services
{
    public interface IMealAddonService
    {
        Task<IEnumerable<ResponseAddonDTO>> GetAllMealAddonsAsync();
        Task<ResponseAddonDTO> GetMealAddonByIdAsync(int id);
        Task<ResponseAddonDTO> CreateMealAddonAsync(RequestAddonDto request);
        Task DeleteMealAddonAsync(int id);
        Task<bool> MealAddonExistsAsync(int id);
    }
}
