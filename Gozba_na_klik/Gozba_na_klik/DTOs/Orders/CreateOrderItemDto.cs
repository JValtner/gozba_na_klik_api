using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Orders
{
    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "MealId je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "MealId mora biti pozitivan broj.")]
        public int MealId { get; set; }

        [Required(ErrorMessage = "Količina je obavezna.")]
        [Range(1, 100, ErrorMessage = "Količina mora biti između 1 i 100.")]
        public int Quantity { get; set; }

        public List<int>? SelectedAddonIds { get; set; }
    }
}
