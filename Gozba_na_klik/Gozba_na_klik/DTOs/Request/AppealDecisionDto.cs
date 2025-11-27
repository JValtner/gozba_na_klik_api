using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Request
{
    public class AppealDecisionDto
    {
        [Required(ErrorMessage = "Odluka je obavezna.")]
        public bool Accept { get; set; }
    }
}

