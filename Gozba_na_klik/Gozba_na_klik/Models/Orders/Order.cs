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
        public string Status { get; set; } = "NA_CEKANJU";
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal SubtotalPrice { get; set; }
        public decimal DeliveryFee { get; set; } = 200m;
        public decimal TotalPrice { get; set; }
        public string? CustomerNote { get; set; }
        public bool HasAllergenWarning { get; set; } = false;
        public List<OrderItem> Items { get; set; } = new();
    }
}
