using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Request
{
    public class RequestUpdateRestaurantByAdminDto
    {
        [Required(ErrorMessage = "Restaurant name is required.")]
        [MaxLength(100, ErrorMessage = "Restaurant name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Restaurant Owner ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Owner ID.")]
        public int OwnerId { get; set; }
    }
}
