using Gozba_na_klik.Enums;
using Gozba_na_klik.Models;
using Gozba_na_klik.Utils;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Repositories
{
    public class MealsDbRepository : IMealsRepository
    {
        private GozbaNaKlikDbContext _context;

        public MealsDbRepository(GozbaNaKlikDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Meal>> GetAllAsync()
        {
            return await _context.Meals
                .Include(m => m.Addons)
                .Include(m => m.Alergens)
                .Include(m => m.Restaurant)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Meal>> GetMealsByRestaurantIdAsync(int restaurantId)
        {
            return await _context.Meals
                .Where(m => m.RestaurantId == restaurantId)
                .Include(m => m.Addons)
                .Include(m => m.Alergens)
                .Include(m => m.Restaurant)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Meal?> GetByIdAsync(int id)
        {
            return await _context.Meals
                .Include(m => m.Addons)
                .Include(m => m.Alergens)
                .Include(m => m.Restaurant)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Meals.AnyAsync(u => u.Id == id);
        }
        public async Task<Meal> AddAsync(Meal meal)
        {
            await _context.Meals.AddAsync(meal);
            await _context.SaveChangesAsync();
            return meal;
        }
        public async Task<Meal> UpdateAsync(Meal meal)
        {
            _context.Meals.Update(meal);
            await _context.SaveChangesAsync();
            return meal;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            Meal meal = await _context.Meals.FindAsync(id);
            if (meal == null)
            {
                return false;
            }

            _context.Meals.Remove(meal);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<PaginatedList<Meal>> GetAllFilteredSortedPagedAsync(
    MealFilter filter, int sortType, int page, int pageSize)
        {
            var baseQuery = _context.Meals
                .Include(m => m.Restaurant)
                .Include(m => m.Addons)
                .Include(m => m.Alergens)
                .AsNoTracking();

            var filtered = FilterMeals(baseQuery, filter);
            var sorted = SortMeals(filtered, sortType);

            int count = await sorted.CountAsync();

            var items = await sorted
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<Meal>(items, count, page, pageSize);
        }

        public IQueryable<Meal> FilterMeals(IQueryable<Meal> query, MealFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(m => m.Name.ToLower().Contains(filter.Name.ToLower()));

            if (filter.MinPrice.HasValue && filter.MinPrice.Value > 0)
                query = query.Where(m => m.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue && filter.MaxPrice.Value > 0)
                query = query.Where(m => m.Price <= filter.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(filter.RestaurantName))
                query = query.Where(m => m.Restaurant != null &&
                                         m.Restaurant.Name.ToLower().Contains(filter.RestaurantName.ToLower()));

            if (filter.Alergens != null && filter.Alergens.Any())
            {
                foreach (var allergen in filter.Alergens)
                {
                    var allergenLower = allergen.ToLower();
                    query = query.Where(m => m.Alergens.Any(a => a.Name.ToLower().Contains(allergenLower)));
                }
            }

            if (filter.Addons != null && filter.Addons.Any())
            {
                foreach (var addon in filter.Addons)
                {
                    var addonLower = addon.ToLower();
                    query = query.Where(m => m.Addons.Any(ad => ad.Name.ToLower().Contains(addonLower)));
                }
            }

            return query;
        }


        public IQueryable<Meal> SortMeals(IQueryable<Meal> query, int sortType)
        {
            return sortType switch
            {
                (int)MealSortType.A_Z => query.OrderBy(m => m.Name),
                (int)MealSortType.Z_A => query.OrderByDescending(m => m.Name),
                (int)MealSortType.Price_Lowest => query.OrderBy(m => m.Price),
                (int)MealSortType.Price_Highest => query.OrderByDescending(m => m.Price),
                (int)MealSortType.Has_Alergens => query.OrderByDescending(m => m.Alergens.Count),
                (int)MealSortType.No_Alergens => query.OrderBy(m => m.Alergens.Count),
                _ => query.OrderBy(m => m.Name)
            };
        }
        public async Task<List<SortTypeOption>> GetSortTypesAsync()
        {
            List<SortTypeOption> options = new List<SortTypeOption>();
            var enumValues = Enum.GetValues(typeof(MealSortType));  // preuzimanje niza vrednosti za enumeraciju
            foreach (MealSortType sortType in enumValues)           // svaku vrednost za enumeraciju konvertuje u SortTypeOption
            {
                options.Add(new SortTypeOption(sortType));
            }
            return options;
        }
    }
}
