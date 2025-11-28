namespace Gozba_na_klik.DTOs.Orders
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string RestaurantName { get; set; }
        public int RestaurantId { get; set; }
        public string DeliveryAddress { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerNote { get; set; }
        public bool HasAllergenWarning { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
        public decimal SubtotalPrice { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPrice { get; set; }
    }
}