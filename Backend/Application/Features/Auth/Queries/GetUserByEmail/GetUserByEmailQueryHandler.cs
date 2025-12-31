
using Application.DTOs;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Queries.GetUser;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserInfo>
{
    private readonly IUserRepository _userRepository;
    public GetUserByEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<UserInfo> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var email = request.Email;
        var user = await _userRepository.GetByEmailAsync(email);
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
