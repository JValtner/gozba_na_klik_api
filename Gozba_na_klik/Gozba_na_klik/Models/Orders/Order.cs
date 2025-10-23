namespace Gozba_na_klik.Models.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        public string Status { get; set; } = "NA_CEKANJU";
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal SubtotalPrice { get; set; }
        public decimal DeliveryFee { get; set; } = 200m;
        public decimal TotalPrice { get; set; }
        public string? CustomerNote { get; set; }
        public bool HasAllergenWarning { get; set; } = false;
        public DateTime? AcceptedAt { get; set; }
        public int? EstimatedPreparationMinutes { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
