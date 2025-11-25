using Microsoft.AspNetCore.Identity;
using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Models
{
    public class User : IdentityUser<int> // Use int if your current Id is int
    {
        // IdentityUser already includes:
        // Id, UserName, PasswordHash, Email, PhoneNumber, EmailConfirmed, etc.
        // Role to be handled through identity framework
        public string? UserImage { get; set; }

        // Adresa
        public int? DefaultAddressId { get; set; }
        public Address? DefaultAddress { get; set; }
        public List<Address> Addresses { get; set; } = new();

        public bool IsActive { get; set; } = true;
        public int? RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
        public List<UserAlergen> UserAlergens { get; set; } = new();

        // Dostava
        public int? ActiveOrderId { get; set; }
        public Order? ActiveOrder { get; set; }

        // Lokacija (kurira)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
