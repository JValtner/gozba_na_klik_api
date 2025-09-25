namespace Gozba_na_klik.Models
{
    public class UpdateUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; } // optional
    }

}
