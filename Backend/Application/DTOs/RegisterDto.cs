using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required.")]
    [Length(6, 50, ErrorMessage = "Password length must be between 6 to 50 charecter")]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = "UserName is required.")]
    [Length(3, 20, ErrorMessage = "UserName length must be between 3 to 20 charecter")]
    public string UserName { get; set; } = string.Empty;

}
