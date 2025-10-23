namespace Gozba_na_klik.DTOs.Orders
{
    public class RestaurantOrderDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string DeliveryAddress { get; set; }
        public string? CustomerNote { get; set; }
        public int ItemsCount { get; set; }
        public decimal TotalPrice { get; set; }
        public List<RestaurantOrderItemDto> Items { get; set; } = new();
        public DateTime? AcceptedAt { get; set; }
        public int? EstimatedPreparationMinutes { get; set; }
    }
}