using Application.DTOs;
using Application.Features.Auth.Queries.GetUser;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserInfo>
{
    private readonly IUserRepository _userRepository;
    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<UserInfo> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var email = request.Id;
        var user = await _userRepository.GetByIdAsync<User>(email);
        if (user is null)
        {
            return null;
        }
        return new UserInfo
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            ProfilePicture = user.ProfilePicture,
            Roles = user.Roles,
            CreatedAt = user.CreatedAt
        };
    }
}
