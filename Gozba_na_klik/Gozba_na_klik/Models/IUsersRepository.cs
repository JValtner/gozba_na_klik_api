namespace Gozba_na_klik.Models
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int userId);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int userId);
        Task<bool> ExistsAsync(int userId);
    }
}
