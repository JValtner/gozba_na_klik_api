using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.Admin;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;
using Gozba_na_klik.Utils;

namespace Gozba_na_klik.Services
{
    public interface IRestaurantService
    {
        Task<List<AdminRestaurantDto>> GetAllRestaurantsAsync();
        Task<Restaurant?> GetRestaurantByIdAsync(int id);
        Task<Restaurant> GetRestaurantByIdOrThrowAsync(int id);
        Task<ResponseRestaurantDTO> GetRestaurantDtoByIdAsync(int id);

        Task<PaginatedList<ResponseRestaurantDTO>> GetAllFilteredSortedPagedAsync(RestaurantFilter filter, int sortType, int page, int pageSize);
        Task<List<SortTypeOption>> GetSortTypesAsync();
        Task<bool> RestaurantExistsAsync(int id);
        Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant);

        // ADMIN
        Task<AdminRestaurantEditDto> GetRestaurantByIdByAdminAsync(int id);
        Task<Restaurant> CreateRestaurantByAdminAsync(RequestCreateRestaurantByAdminDto dto);
        Task<Restaurant> UpdateRestaurantByAdminAsync(int id,RequestUpdateRestaurantByAdminDto dto);

        Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant);
        Task<Restaurant> UpdateRestaurantByOwnerAsync(int id, RestaurantUpdateDto dto, int ownerId);
        Task DeleteRestaurantAsync(int id);
        Task<IEnumerable<Restaurant>> GetRestaurantsByOwnerAsync(int ownerId);
        Task UpdateWorkSchedulesFromDtosAsync(int restaurantId, List<WorkScheduleDto> scheduleDtos);
        Task UpdateWorkSchedulesAsync(int restaurantId, List<WorkSchedule> schedules);
        Task AddClosedDateAsync(int restaurantId, ClosedDate date);
        Task RemoveClosedDateAsync(int restaurantId, int dateId);
        Task<List<IrresponsibleRestaurantDto>> GetIrresponsibleRestaurantsAsync();
        Task<SuspensionResponseDto> SuspendRestaurantAsync(int restaurantId, string reason, int adminId);
        Task<SuspensionResponseDto?> GetRestaurantSuspensionAsync(int restaurantId);
        Task<SuspensionResponseDto> AppealSuspensionAsync(int restaurantId, string appealText, int ownerId);
        Task<List<SuspensionResponseDto>> GetAppealedSuspensionsAsync();
        Task ProcessAppealDecisionAsync(int restaurantId, bool accept, int adminId);
    }
}
