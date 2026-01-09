using Application.Features.Auth.Commands.Register;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Auth.DTOs;

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

    [Required(ErrorMessage = "Phone number is required.")]
    //[Phone(ErrorMessage = "Invalid phone number.")]
    public string PhoneNumber { get; set; } = string.Empty;

    public RegisterUserCommand ToRegisterUserCommand()
    {
        return new RegisterUserCommand()
        {
            Email = this.Email,
            Password = this.Password,
            UserName = this.UserName,
            PhoneNumber = this.PhoneNumber
        };
    }
}
