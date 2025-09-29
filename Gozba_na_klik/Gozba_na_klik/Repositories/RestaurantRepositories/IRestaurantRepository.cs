using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Restaurants;

namespace Gozba_na_klik.Repositories.RestaurantRepositories;

public interface IRestaurantRepository
{
    Task<IEnumerable<Restaurant>> GetAllAsync();
    Task<Restaurant?> GetByIdAsync(int id);
    Task<Restaurant> AddAsync(Restaurant restaurant);
    Task<Restaurant> UpdateAsync(Restaurant restaurant);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<IEnumerable<Restaurant>> GetByOwnerAsync(int ownerId);
}
