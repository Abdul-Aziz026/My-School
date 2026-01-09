
using MediatR;
using Domain.Entities;
using Application.Features.Auth.DTOs;
using Application.Common.Interfaces.Repositories;
using Application.Features.Users.DTOs;
using Application.Common.Helper;

namespace Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDtoResponse>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<CurrentUserDtoResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var response = new CurrentUserDtoResponse();
        var currentUserContext = TellMe.GetCurrentUserContext();
        if (currentUserContext is null || string.IsNullOrWhiteSpace(currentUserContext.UserId))
        {
            response.IsSuccess = false;
            response.ErrorMessage = "User not authenticated or token is invalid";
        }

        // Fetch user from database
        var user = await _userRepository.GetByIdAsync<User>(currentUserContext?.UserId!);
        if (user is null)
        {
            response.IsSuccess = false;
            response.ErrorMessage = "User not found";
            return response;
        }

        response.IsSuccess = true;
        response.User = user.ToUserDtoResponse();
        return response;
    }
}


