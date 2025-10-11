using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Request
{
    public class RequestAlergenDto
    {
        [Required(ErrorMessage = "Allergen name is required.")]
        [MaxLength(100, ErrorMessage = "Allergen name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Meal ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid meal ID.")]
        public int MealId { get; set; }
    }
}
