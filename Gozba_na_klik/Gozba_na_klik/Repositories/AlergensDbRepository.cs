using Gozba_na_klik.Models;
using Microsoft.EntityFrameworkCore;
using Gozba_na_klik.Exceptions;

namespace Gozba_na_klik.Repositories
{
    public class AlergensDbRepository : IAlergensRepository
    {
        private readonly GozbaNaKlikDbContext _context;
        private readonly IMealsRepository _mealsRepository;

        public AlergensDbRepository(GozbaNaKlikDbContext context, IMealsRepository mealsRepository)
        {
            _context = context;
            _mealsRepository = mealsRepository;
        }

        public async Task<IEnumerable<Alergen>> GetAllAsync()
        {
            return await _context.Alergens.ToListAsync();
        }

        public async Task<Alergen?> GetByIdAsync(int id)
        {
            return await _context.Alergens.FindAsync(id);
        }

        public async Task<IEnumerable<Alergen>> GetAlergenByMealIdAsync(int mealId)
        {
            return await _context.Alergens
                .Where(a => a.MealId == mealId)
                .ToListAsync();
        }

        public async Task<Alergen?> AddAlergenToMealAsync(int mealId, int alergenId)
        {
            var meal = await _mealsRepository.GetByIdAsync(mealId)
                ?? throw new NotFoundException($"Meal with ID {mealId} not found.");

            var alergen = await _context.Alergens.FindAsync(alergenId)
                ?? throw new NotFoundException($"Alergen with ID {alergenId} not found.");

            if (meal.Alergens.Any(a => a.Id == alergenId))
                throw new BadRequestException($"Meal {mealId} already contains allergen {alergenId}.");

            meal.Alergens.Add(alergen);
            await _context.SaveChangesAsync();
            return alergen;
        }

        public async Task<Alergen?> RemoveAlergenFromMealAsync(int mealId, int alergenId)
        {
            var meal = await _mealsRepository.GetByIdAsync(mealId)
                ?? throw new NotFoundException($"Meal with ID {mealId} not found.");

            var alergen = await _context.Alergens.FindAsync(alergenId)
                ?? throw new NotFoundException($"Alergen with ID {alergenId} not found.");

            if (!meal.Alergens.Any(a => a.Id == alergenId))
                throw new BadRequestException($"Meal {mealId} doesn’t contain allergen {alergenId}.");

            meal.Alergens.Remove(alergen);
            await _context.SaveChangesAsync();
            return alergen;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Alergens.AnyAsync(a => a.Id == id);
        }
    }
}
