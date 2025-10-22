using Gozba_na_klik.DTOs.Orders;

namespace Gozba_na_klik.DTOs.Orders
{
    public class OrderHistoryResponseDto
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryAddress { get; set; }
        public string? CustomerNote { get; set; }
        public bool HasAllergenWarning { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }

    public class PaginatedOrderHistoryResponseDto
    {
        public List<OrderHistoryResponseDto> Orders { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }
}