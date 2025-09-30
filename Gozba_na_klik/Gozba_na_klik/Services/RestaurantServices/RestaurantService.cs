using Gozba_na_klik.Models;
using Gozba_na_klik.Models.RestaurantModels;
using Gozba_na_klik.Models.Restaurants;
using Gozba_na_klik.Repositories;
using Gozba_na_klik.Repositories.RestaurantRepositories;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Services.RestaurantServices
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly GozbaNaKlikDbContext _context;

        public RestaurantService(IRestaurantRepository restaurantRepository, GozbaNaKlikDbContext context)
        {
            _restaurantRepository = restaurantRepository;
            _context = context;
        }

        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
        {
            return await _restaurantRepository.GetAllAsync();
        }

        public async Task<Restaurant?> GetRestaurantByIdAsync(int id)
        {
            return await _restaurantRepository.GetByIdAsync(id);
        }

        public async Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant)
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

        public async Task<IEnumerable<Restaurant>> GetRestaurantsByOwnerAsync(int ownerId)
        {
            return await _restaurantRepository.GetByOwnerAsync(ownerId);
        }

        public async Task UpdateWorkSchedulesAsync(int restaurantId, List<WorkSchedule> schedules)
        {
            Restaurant? restaurant = await _context.Restaurants
                .Include(r => r.WorkSchedules)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null) return;

            _context.WorkSchedules.RemoveRange(restaurant.WorkSchedules);

            foreach (WorkSchedule schedule in schedules)
            {
                schedule.RestaurantId = restaurantId;
                _context.WorkSchedules.Add(schedule);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddClosedDateAsync(int restaurantId, ClosedDate date)
        {
            date.RestaurantId = restaurantId;
            _context.ClosedDates.Add(date);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveClosedDateAsync(int restaurantId, int dateId)
        {
            ClosedDate? closedDate = await _context.ClosedDates
                .FirstOrDefaultAsync(cd => cd.Id == dateId && cd.RestaurantId == restaurantId);

            if (closedDate != null)
            {
                _context.ClosedDates.Remove(closedDate);
                await _context.SaveChangesAsync();
            }
        }
    }
}