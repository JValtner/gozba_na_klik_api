using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Orders
{
    public class AcceptOrderDto
    {
        [Required(ErrorMessage = "Procenjeno vreme pripreme je obavezno.")]
        [Range(5, 180, ErrorMessage = "Vreme pripreme mora biti između 5 i 180 minuta.")]
        public int EstimatedPreparationMinutes { get; set; }
    }
}