using System.ComponentModel.DataAnnotations;

namespace Gozba_na_klik.DTOs.Request
{
    public class RequestUpdateUserByAdminDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "User role is required")]
        public string Role { get; set; } = string.Empty;
    }
}
