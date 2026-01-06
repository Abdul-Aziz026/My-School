using Application.Features.Users.Commands.CreateUser;

namespace Application.Features.Users.DTOs;

public class CreateUserDtoRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public string? Address { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();

    public CreateUserCommand ToCreateUserCommand()
    {
        return new CreateUserCommand
        {
            Email = this.Email,
            Password = this.Password,
            UserName = this.UserName,
            PhoneNumber = this.PhoneNumber,
            ProfilePicture = this.ProfilePicture,
            Address = this.Address,
            Roles = this.Roles,
            Permissions = this.Permissions
        };
    }
}
