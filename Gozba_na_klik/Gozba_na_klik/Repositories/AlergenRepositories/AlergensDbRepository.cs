using Gozba_na_klik.Models;
using Gozba_na_klik.Models.MealModels;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Repositories.AlergenRepositories
{
    public class AlergensDbRepository
    {
        private GozbaNaKlikDbContext _context;

        public AlergensDbRepository(GozbaNaKlikDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Alergen>> GetAllAsync()
        {
            return await _context.Alergens.ToListAsync();
        }
        public async Task<Alergen?> GetByIdAsync(int id)
        {
            return await _context.Alergens.FindAsync(id);
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Alergens.AnyAsync(a => a.Id == id);
        }
        public async Task<Alergen> AddAsync(Alergen alergen)
        {
            await _context.Alergens.AddAsync(alergen);
            await _context.SaveChangesAsync();
            return alergen;
        }
        public async Task<Alergen> UpdateAsync(Alergen alergen)
        {
            _context.Alergens.Update(alergen);
            await _context.SaveChangesAsync();
            return alergen;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            Alergen alergen = await _context.Alergens.FindAsync(id);
            if (alergen == null)
            {
                return false;
            }

            _context.Alergens.Remove(alergen);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
