using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Models
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByIdWithAlergensAsync(int userId);
        Task<List<User>> GetAllAvailableCouriersAsync();
        Task<IEnumerable<User>> GetAllRestaurantOwnersAsync();
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<User?> UpdateUserAlergensAsync(int userId, List<int> alergenIds);
        Task<User?> AssignOrderToCourier(Order order, User courier);
        Task<bool> DeleteAsync(int userId);
        Task<bool> ExistsAsync(int userId);
    }
}
