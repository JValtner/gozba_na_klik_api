using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Restaurants;
using Gozba_na_klik.Repositories;
using Gozba_na_klik.Repositories.RestaurantRepositories;

namespace Gozba_na_klik.Services.RestaurantServices
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
        {
            return await _restaurantRepository.GetAllAsync();
        }

        public async Task<Restaurant?> GetRestaurantByIdAsync(int id)
        {
            return await _restaurantRepository.GetByIdAsync(id);
        }

        public async Task<Restaurant> CreateUserAsync(Restaurant restaurant)
        {
            return await _restaurantRepository.AddAsync(restaurant);
        }

        public async Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant)
        {
            return await _restaurantRepository.UpdateAsync(restaurant);
        }

        public async Task DeleteRestaurantAsync(int id)
        {
            await _restaurantRepository.DeleteAsync(id);
        }

        public async Task<bool> RestaurantExistsAsync(int id)
        {
            return await _restaurantRepository.ExistsAsync(id);
        }

        public Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant)
        {
            throw new NotImplementedException();
        }
    }
}
