using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Request
{
    public class RequestMealDto
    {
        [Required(ErrorMessage = "Meal name is required.")]
        [MaxLength(100, ErrorMessage = "Meal name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Restaurant ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid restaurant ID.")]
        public int RestaurantId { get; set; }
    }
}
