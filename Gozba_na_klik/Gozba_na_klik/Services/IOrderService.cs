using Gozba_na_klik.DTOs.Orders;

namespace Gozba_na_klik.Services
{
    public interface IOrderService
    {
        Task<OrderPreviewDto> GetOrderPreviewAsync(int userId, int restaurantId, CreateOrderDto dto);
        Task<OrderResponseDto> CreateOrderAsync(int userId, int restaurantId, CreateOrderDto dto);
    }
}
