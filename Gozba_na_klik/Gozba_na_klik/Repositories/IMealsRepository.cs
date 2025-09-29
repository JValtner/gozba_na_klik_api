using Gozba_na_klik.Models;

namespace Gozba_na_klik.Repositories
{
    public interface IMealsRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int userId);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int userId);

        Task<bool> ExistsAsync(int userId);
    }
}
