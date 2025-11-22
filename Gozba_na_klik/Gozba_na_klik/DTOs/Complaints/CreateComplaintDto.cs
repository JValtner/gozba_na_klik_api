using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Complaints
{
    public class CreateComplaintDto
    {
        [Required(ErrorMessage = "OrderId je obavezan.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Tekst žalbe je obavezan.")]
        [MinLength(10, ErrorMessage = "Žalba mora imati najmanje 10 karaktera.")]
        public string Message { get; set; } = string.Empty;
    }
}

