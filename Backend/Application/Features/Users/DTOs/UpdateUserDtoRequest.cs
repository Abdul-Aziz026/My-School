using Application.Features.Users.Commands.UpdateUser;

namespace Application.Features.Users.DTOs;

public class UpdateUserDtoRequest
{
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public UpdateUserCommand ToUpdateUserCommand(string UserId)
    {
        return new UpdateUserCommand
        {
            UserId = UserId,
            UserName = this.UserName,
            PhoneNumber = this.PhoneNumber,
            ProfilePicture = this.ProfilePicture,
            Address = this.Address
        };
    }
}
