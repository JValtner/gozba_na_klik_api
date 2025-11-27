namespace Gozba_na_klik.Models.Orders
{
    public class Order
    {
        public int Id { get; set; }

        // Kupac (User)
        public int UserId { get; set; }
        public User User { get; set; }

        // Dostavljac (Delivery person)
        public int? DeliveryPersonId { get; set; }
        public User? DeliveryPerson { get; set; }

        // Restoran
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

        public int? AddressId { get; set; }
        public Address? Address { get; set; }
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
        public DateTime? PickupTime { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public List<OrderItem> Items { get; set; } = new();

        public string Status { get; set; } = "NA ČEKANJU";
        // STATUSI U BAZI:
        //   "NA ČEKANJU" 
        //   "OTKAZANA" 
        //   "PRIHVAĆENA" 
        //   "PREUZIMANJE U TOKU" 
        //   "DOSTAVA U TOKU" 
        //   "ZAVRŠENO"
    }
}
