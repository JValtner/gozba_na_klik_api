using Gozba_na_klik.Utils;

namespace Gozba_na_klik.DTOs.Orders
{
    public class CourierDeliveryHistoryItemDto
    {
        public int OrderId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public DateTime? PickupTime { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public int? DurationMinutes { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

