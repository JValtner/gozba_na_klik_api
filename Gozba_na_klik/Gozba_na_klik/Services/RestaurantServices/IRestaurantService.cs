using Gozba_na_klik.Models;
using Gozba_na_klik.Models.RestaurantModels;
using Gozba_na_klik.Models.Restaurants;

namespace Gozba_na_klik.Services.RestaurantServices
{
    public interface IRestaurantService
    {
        Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();
        Task<Restaurant?> GetRestaurantByIdAsync(int id);
        Task<bool> RestaurantExistsAsync(int id);
        Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant);
        Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant);
        Task DeleteRestaurantAsync(int id);
        Task<IEnumerable<Restaurant>> GetRestaurantsByOwnerAsync(int ownerId);
        Task UpdateWorkSchedulesAsync(int restaurantId, List<WorkSchedule> schedules);
        Task AddClosedDateAsync(int restaurantId, ClosedDate date);
        Task RemoveClosedDateAsync(int restaurantId, int dateId);
    }
}
