using System;
using System.Linq;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Repositories
{
    public class UsersDbRepository: IUsersRepository
    {
        private GozbaNaKlikDbContext _context;
        private UserManager<User> _userManager;

        public UsersDbRepository(GozbaNaKlikDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
        // Vlasnici restorana
        public async Task<IEnumerable<User>> GetAllRestaurantOwnersAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("RestaurantOwner");
            return users;
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        // Dobavljanje Korisnika sa aloergenima
        public async Task<User?> GetByIdWithAlergensAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserAlergens)
                    .ThenInclude(ua => ua.Alergen)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<int>> GetAvailableCourierIdsAsync(DayOfWeek day, TimeSpan now)
        {
            return await _context.DeliveryPersonSchedules
                .Where(s => s.IsActive
                            && s.DayOfWeek == day
                            && s.StartTime <= now
                            && s.EndTime >= now)
                .Select(s => s.DeliveryPersonId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<User>> GetCouriersByIdsAsync(List<int> ids)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => ids.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }
        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
        // Update korisnikovih alergena
        public async Task<User?> UpdateUserAlergensAsync(int userId, List<int> alergenIds)
        {
            var user = await GetByIdWithAlergensAsync(userId);
            if (user == null) return null;

            // Zamenimo listu alergena
            user.UserAlergens = alergenIds
                .Distinct()
                .Select(id => new UserAlergen { UserId = userId, AlergenId = id })
                .ToList();

            await _context.SaveChangesAsync();
            return user;
        }
       
        public async Task<bool> DeleteAsync(int id)
        {
            User user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<string?> GetUserRoleAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault(); // assuming 1 role per user
        }
    }
}
