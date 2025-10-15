using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;

namespace Gozba_na_klik.Services
{
    public interface IMealAddonService
    {
        Task<IEnumerable<ResponseAddonDTO>> GetAddonsByMealIdAsync(int mealId);
        Task<ResponseAddonDTO> GetMealAddonByIdAsync(int id);
        Task<ResponseAddonDTO> CreateMealAddonAsync(RequestAddonDto request);
        Task SetActiveChosenAddon(int addonId);
        Task DeleteMealAddonAsync(int id);
        Task<bool> MealAddonExistsAsync(int id);
    }
}
