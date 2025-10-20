using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Orders
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "AddressId je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Morate izabrati adresu dostave.")]
        public int AddressId { get; set; }

        public string? CustomerNote { get; set; }

        [Required(ErrorMessage = "Stavke porudžbine su obavezne.")]
        [MinLength(1, ErrorMessage = "Porudžbina mora imati bar jednu stavku.")]
        public List<CreateOrderItemDto> Items { get; set; } = new();

        public bool AllergenWarningAccepted { get; set; } = false;
    }
}