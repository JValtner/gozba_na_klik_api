namespace Gozba_na_klik.DTOs.Orders
{
    public class OrderPreviewDto
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public bool IsRestaurantOpen { get; set; }
        public string? ClosedReason { get; set; }
        public List<OrderItemPreviewDto> Items { get; set; } = new();
        public decimal SubtotalPrice { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPrice { get; set; }
        public bool HasAllergens { get; set; }
        public List<string> Allergens { get; set; } = new();
    }
}
