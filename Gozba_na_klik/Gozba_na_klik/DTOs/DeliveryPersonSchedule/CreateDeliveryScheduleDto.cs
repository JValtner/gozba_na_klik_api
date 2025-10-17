using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.DeliveryPersonSchedule
{
    public class CreateDeliveryScheduleDto
    {
        [Required(ErrorMessage = "Dan u nedelji je obavezan.")]
        [Range(0, 6, ErrorMessage = "Dan u nedelji mora biti između 0 (Nedelja) i 6 (Subota).")]
        public int DayOfWeek { get; set; }

        [Required(ErrorMessage = "Početno vreme je obavezno.")]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Početno vreme mora biti u formatu HH:mm (npr. 09:00).")]
        public string StartTime { get; set; }

        [Required(ErrorMessage = "Krajnje vreme je obavezno.")]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Krajnje vreme mora biti u formatu HH:mm (npr. 17:00).")]
        public string EndTime { get; set; }
    }
}