using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;
using Gozba_na_klik.Utils;

namespace Gozba_na_klik.Services
{
    public interface IRestaurantService
    {
        Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();
        Task<Restaurant?> GetRestaurantByIdAsync(int id);

        Task<PaginatedList<ResponseRestaurantDTO>> GetAllFilteredSortedPagedAsync(RestaurantFilter filter, int sortType, int page, int pageSize);
        Task<List<SortTypeOption>> GetSortTypesAsync();
        Task<bool> RestaurantExistsAsync(int id);
        Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant);

        // ADMIN
        Task<Restaurant> CreateRestaurantByAdminAsync(RequestCreateRestaurantByAdminDto dto);
        Task<Restaurant> UpdateRestaurantByAdminAsync(int id,RequestUpdateRestaurantByAdminDto dto);

        Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant);
        Task DeleteRestaurantAsync(int id);
        Task<IEnumerable<Restaurant>> GetRestaurantsByOwnerAsync(int ownerId);
        Task UpdateWorkSchedulesAsync(int restaurantId, List<WorkSchedule> schedules);
        Task AddClosedDateAsync(int restaurantId, ClosedDate date);
        Task RemoveClosedDateAsync(int restaurantId, int dateId);
    }
}
