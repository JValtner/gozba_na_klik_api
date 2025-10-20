using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Models
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int orderId);
        Task<Order> AddAsync(Order order);
        Task<bool> ExistsAsync(int orderId);
    }
}
