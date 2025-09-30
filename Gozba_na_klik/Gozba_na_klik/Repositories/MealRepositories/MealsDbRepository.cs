using System;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.MealModels;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Repositories.MealRepositories
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
    }
}
