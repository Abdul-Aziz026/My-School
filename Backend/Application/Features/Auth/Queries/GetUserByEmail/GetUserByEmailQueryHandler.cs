
using Application.Common.Interfaces.Repositories;
using Application.Features.Auth.DTOs;
using Application.Features.Users.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Queries.GetUser;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDtoResponse>
{
    private readonly IUserRepository _userRepository;
    public GetUserByEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<UserDtoResponse> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var response = new UserDtoResponse();
        var email = request.Email;
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            return null!;
        }
        return user.ToUserDtoResponse();
    }
}
