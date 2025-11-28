using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Request
{
    public class AppealSuspensionDto
    {
        [Required(ErrorMessage = "Tekst žalbe je obavezan.")]
        [MinLength(10, ErrorMessage = "Tekst žalbe mora imati najmanje 10 karaktera.")]
        [MaxLength(500, ErrorMessage = "Tekst žalbe može imati najviše 500 karaktera.")]
        public string AppealText { get; set; } = string.Empty;
    }
}

