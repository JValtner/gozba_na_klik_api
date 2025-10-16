namespace Gozba_na_klik.DTOs
{
 
    public class EmployeeListItemDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string? UserImage { get; set; }
    }
}