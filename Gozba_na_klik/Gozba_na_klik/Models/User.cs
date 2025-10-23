using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // ULOGE IZ BAZE => "User", "Admin", "RestaurantOwner", "RestaurantEmployee", "DeliveryPerson"
        public string? UserImage { get; set; }

        // Adresa
        public int? DefaultAddressId { get; set; }
        public Address? DefaultAddress { get; set; } // Ovo je default address
        public List<Address> Addresses { get; set; } = new();

        public bool IsActive { get; set; } = true; // Da li je zaposlen aktivan/suspendovan
        public int? RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
        public List<UserAlergen> UserAlergens { get; set; } = new();
        
        // Dostava
        public int? ActiveOrderId { get; set; }
        public Order? ActiveOrder { get; set; }
    }
}
