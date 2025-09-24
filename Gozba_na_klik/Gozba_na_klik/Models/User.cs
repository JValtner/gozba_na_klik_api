namespace Gozba_na_klik.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // e.g., "Client", "Admin", "RestaurantOwner", "RestaurantEmployee", "DeliveryPerson"
<<<<<<< Updated upstream
        public string ProfileImgPath { get; set; } // path to profile image
=======
        public string? UserImage { get; set; }
>>>>>>> Stashed changes
    }
}
