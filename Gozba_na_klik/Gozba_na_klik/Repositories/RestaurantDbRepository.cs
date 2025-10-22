using Gozba_na_klik.Enums;
using Gozba_na_klik.Models;
using Microsoft.EntityFrameworkCore;
using Gozba_na_klik.Utils;

namespace Gozba_na_klik.Repositories;

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
    public async Task<PaginatedList<Restaurant>> GetAllFilteredSortedPagedAsync(RestaurantFilter filter, int sortType, int page, int pageSize)
    {
        IQueryable<Restaurant> query = _context.Restaurants
            .Include(r => r.Owner)
            .Include(r => r.WorkSchedules)
            .Include(r => r.ClosedDates);
        //Disabled zbog testiranja ne prikazuje restorane za koje nema definisano radno vreme tj smatra da ne rade
        //query = FilterRestaurants(query, filter);
        query = SortRestaurants(query, sortType);

        int count = await query.CountAsync();
        int skip = (page - 1) * pageSize;
        List<Restaurant> items = await query.Skip(skip).Take(pageSize).ToListAsync();

        return new PaginatedList<Restaurant>(items, count, page, pageSize);
    }

    private static IQueryable<Restaurant> FilterRestaurants(IQueryable<Restaurant> query, RestaurantFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.Name))
            query = query.Where(r => r.Name.ToLower().Contains(filter.Name.ToLower()));

        if (!string.IsNullOrEmpty(filter.Address))
            query = query.Where(r => r.Address != null && r.Address.ToLower().Contains(filter.Address.ToLower()));

        if (filter.CurrentDate.HasValue)
        {
            var currentDate = DateTime.SpecifyKind(filter.CurrentDate.Value, DateTimeKind.Utc);
            var currentDay = currentDate.DayOfWeek;
            var currentTime = currentDate.TimeOfDay;

            query = query.Where(r =>
                !r.ClosedDates.Any(cd =>
                    DateTime.SpecifyKind(cd.Date, DateTimeKind.Utc).Date == currentDate.Date) &&
                r.WorkSchedules.Any(ws =>
                    ws.DayOfWeek == currentDay &&
                    ws.OpenTime <= currentTime &&
                    ws.CloseTime >= currentTime));
        }

        return query;
    }

    private static IQueryable<Restaurant> SortRestaurants(IQueryable<Restaurant> query, int sortType)
    {
        return sortType switch
        {
            (int)RestaurantSortType.A_Z => query.OrderBy(m => m.Name),
            (int)RestaurantSortType.Z_A => query.OrderByDescending(m => m.Name),
            _ => query.OrderBy(m => m.Name)
        };
    }
    public async Task<List<SortTypeOption>> GetSortTypesAsync()
    {
        List<SortTypeOption> options = new List<SortTypeOption>();
        var enumValues = Enum.GetValues(typeof(RestaurantSortType));  // preuzimanje niza vrednosti za enumeraciju
        foreach (RestaurantSortType sortType in enumValues)           // svaku vrednost za enumeraciju konvertuje u SortTypeOption
        {
            options.Add(new SortTypeOption(sortType));
        }
        return options;
    }
}
