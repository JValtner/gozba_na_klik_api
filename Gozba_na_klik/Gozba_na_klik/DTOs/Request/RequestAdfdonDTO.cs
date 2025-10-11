using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Request
{
    public class RequestAddonDto
    {
        [Required(ErrorMessage = "Addon name is required.")]
        [MaxLength(100, ErrorMessage = "Addon name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 5000, ErrorMessage = "Price must be a valid positive amount.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Meal ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid meal ID.")]
        public int MealId { get; set; }
    }
}
