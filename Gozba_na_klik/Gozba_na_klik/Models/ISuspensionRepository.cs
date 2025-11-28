using Gozba_na_klik.DTOs.Response;

namespace Gozba_na_klik.Models
{
    public interface ISuspensionRepository
    {
        Task<SuspensionResponseDto> InsertSuspensionAsync(int restaurantId, string reason, int adminId);
        Task<SuspensionResponseDto?> GetSuspensionByRestaurantIdAsync(int restaurantId);
        Task<SuspensionResponseDto?> UpdateSuspensionWithAppealAsync(int restaurantId, string appealText, int ownerId);
        Task<SuspensionResponseDto?> UpdateSuspensionDecisionAsync(int restaurantId, bool accept, int adminId);
        Task<bool> DeleteSuspensionAsync(int restaurantId);
        Task<List<SuspensionResponseDto>> GetAppealedSuspensionsAsync();
        Task<SuspensionResponseDto?> GetAppealedSuspensionByRestaurantIdAsync(int restaurantId);
    }
}

