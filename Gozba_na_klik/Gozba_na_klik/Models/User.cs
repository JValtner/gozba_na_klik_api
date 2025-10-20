using Gozba_na_klik.Models;

namespace Gozba_na_klik.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // e.g., "Client", "Admin", "RestaurantOwner", "RestaurantEmployee", "DeliveryPerson"
        public string? UserImage { get; set; }
        public bool IsActive { get; set; } = true;
        public int? RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
        public List<UserAlergen> UserAlergens { get; set; } = new();
    }
}
