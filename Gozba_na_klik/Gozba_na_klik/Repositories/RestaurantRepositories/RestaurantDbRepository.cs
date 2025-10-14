using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Restaurants;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Gozba_na_klik.Repositories.RestaurantRepositories;

public class RestaurantDbRepository : IRestaurantRepository
{
    private GozbaNaKlikDbContext _context;

    public RestaurantDbRepository(GozbaNaKlikDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Restaurant>> GetAllAsync()
    {
        return await _context.Restaurants
                //.Include(r => r.Owner)
                //.Include(r => r.Menu)
                //    .ThenInclude(m => m.Addons)
                //.Include(r => r.Menu)
                //    .ThenInclude(m => m.Alergens)
                .Include(r => r.WorkSchedules)
                .Include(r => r.ClosedDates)
                .ToListAsync();
    }
    public async Task<Restaurant?> GetByIdAsync(int id)
    {
        return await _context.Restaurants
                //.Include(r => r.Owner)
                //.Include(r => r.Menu)
                //    .ThenInclude(m => m.Addons)
                //.Include(r => r.Menu)
                //    .ThenInclude(m => m.Alergens)
                .Include(r => r.WorkSchedules)
                .Include(r => r.ClosedDates)
                .FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Restaurants.AnyAsync(u => u.Id == id);
    }
    public async Task<Restaurant> AddAsync(Restaurant restaurant)
    {
        await _context.Restaurants.AddAsync(restaurant);
        await _context.SaveChangesAsync();
        return restaurant;
    }
    public async Task<Restaurant> UpdateAsync(Restaurant restaurant)
    {
        _context.Restaurants.Update(restaurant);
        await _context.SaveChangesAsync();
        return restaurant;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        Restaurant restaurant = await _context.Restaurants.FindAsync(id);
        if (restaurant == null)
        {
            return false;
        }

        _context.Restaurants.Remove(restaurant);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<Restaurant>> GetByOwnerAsync(int ownerId)
    {
        return await _context.Restaurants
                //.Include(r => r.Owner)
                //.Include(r => r.Menu)
                //    .ThenInclude(m => m.Addons)
                //.Include(r => r.Menu)
                //    .ThenInclude(m => m.Alergens)
                .Include(r => r.WorkSchedules)
                .Include(r => r.ClosedDates)
                .Where(r => r.OwnerId == ownerId)
                .OrderBy(r => r.Name)
                .ToListAsync();
    }
}
