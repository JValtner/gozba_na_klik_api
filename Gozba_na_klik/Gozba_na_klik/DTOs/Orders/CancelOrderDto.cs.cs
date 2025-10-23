using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Orders
{
    public class CancelOrderDto
    {
        [Required(ErrorMessage = "Razlog otkazivanja je obavezan.")]
        [MinLength(5, ErrorMessage = "Razlog mora imati najmanje 5 karaktera.")]
        [MaxLength(500, ErrorMessage = "Razlog ne može biti duži od 500 karaktera.")]
        public string Reason { get; set; }
    }
}