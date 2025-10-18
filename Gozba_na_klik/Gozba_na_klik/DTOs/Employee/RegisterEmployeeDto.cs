using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs
{
    public class RegisterEmployeeDto
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisničko ime mora biti između 3 i 50 karaktera")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Email adresa nije validna")]
        [StringLength(100, ErrorMessage = "Email ne može biti duži od 100 karaktera")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Lozinka mora biti između 6 i 100 karaktera")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Uloga je obavezna")]
        [RegularExpression("^(RestaurantEmployee|DeliveryPerson)$",
            ErrorMessage = "Uloga mora biti 'RestaurantEmployee' ili 'DeliveryPerson'")]
        public string Role { get; set; }
    }
}