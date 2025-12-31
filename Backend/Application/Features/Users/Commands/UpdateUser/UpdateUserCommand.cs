using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest
{
    public string UserId { get; set; }
    public UpdateUserProfileDto UpdateUserProfileDto { get; }
    public UpdateUserCommand(UpdateUserProfileDto updateUserProfile)
    {
        UpdateUserProfileDto = updateUserProfile;
    }
}
