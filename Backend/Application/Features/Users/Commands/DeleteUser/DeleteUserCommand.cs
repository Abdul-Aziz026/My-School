
using MediatR;

namespace Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest
{
    public string UserId { get; set; }
    public DeleteUserCommand(string userId)
    {
        UserId = userId;
    }
}
