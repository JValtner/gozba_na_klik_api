using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Request
{
    public class SuspendRestaurantDto
    {
        [Required(ErrorMessage = "Razlog suspenzije je obavezan.")]
        [MinLength(10, ErrorMessage = "Razlog suspenzije mora imati najmanje 10 karaktera.")]
        [MaxLength(500, ErrorMessage = "Razlog suspenzije može imati najviše 500 karaktera.")]
        public string Reason { get; set; } = string.Empty;
    }
}

