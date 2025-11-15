namespace Gozba_na_klik.DTOs.Request
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegistrationDto
    {
        
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
    public class ProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserImage { get; set; }
        
    }


}
