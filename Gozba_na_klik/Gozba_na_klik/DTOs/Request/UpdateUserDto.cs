using System.ComponentModel.DataAnnotations;

public class UpdateUserDto
{
    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one letter and one number.")]
    public string? Password { get; set; } // optional
}
