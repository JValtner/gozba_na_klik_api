using System;
using Gozba_na_klik.Models;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Repositories
{
    public class MealAddonsDbRepository : IMealAddonsRepository

    {
        private GozbaNaKlikDbContext _context;

        public MealAddonsDbRepository(GozbaNaKlikDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<MealAddon>> GetAllAsync()
        {
            return await _context.MealAddons.ToListAsync();
        }
        public async Task<MealAddon?> GetByIdAsync(int id)
        {
            return await _context.MealAddons.FindAsync(id);
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MealAddons.AnyAsync(u => u.Id == id);
        }
        public async Task<MealAddon> AddAsync(MealAddon mealAddon)
        {
            await _context.MealAddons.AddAsync(mealAddon);
            await _context.SaveChangesAsync();
            return mealAddon;
        }
        public async Task<MealAddon> UpdateAsync(MealAddon mealAddon)
        {
            _context.MealAddons.Update(mealAddon);
            await _context.SaveChangesAsync();
            return mealAddon;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            MealAddon mealAddon = await _context.MealAddons.FindAsync(id);
            if (mealAddon == null)
            {
                return false;
            }

            _context.MealAddons.Remove(mealAddon);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
