using Gozba_na_klik.Models;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Repositories
{
    public class MealAddonsDbRepository : IMealAddonsRepository
    {
        private readonly GozbaNaKlikDbContext _context;

        public MealAddonsDbRepository(GozbaNaKlikDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MealAddon>> GetAllByMealIdAsync(int mealId)
        {
            return await _context.MealAddons
                .Where(a => a.MealId == mealId)
                .ToListAsync();
        }

        public async Task<MealAddon?> GetByIdAsync(int id)
        {
            return await _context.MealAddons.FindAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MealAddons.AnyAsync(a => a.Id == id);
        }

        public async Task<MealAddon> AddAsync(MealAddon mealAddon)
        {
            await _context.MealAddons.AddAsync(mealAddon);
            await _context.SaveChangesAsync();
            return mealAddon;
        }

        public async Task<bool> SetActiveChosenAddon(int addonId)
        {
            var addon = await _context.MealAddons.FindAsync(addonId);
            if (addon == null || addon.Type != "chosen") return false;

            var chosenAddons = await _context.MealAddons
                .Where(a => a.MealId == addon.MealId && a.Type == "chosen")
                .ToListAsync();

            foreach (var a in chosenAddons)
                a.IsActive = a.Id == addonId;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var mealAddon = await _context.MealAddons.FindAsync(id);
            if (mealAddon == null) return false;

            _context.MealAddons.Remove(mealAddon);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
