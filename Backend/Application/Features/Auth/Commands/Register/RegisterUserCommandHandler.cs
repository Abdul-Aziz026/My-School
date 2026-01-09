
using Application.Features.Auth.DTOs;
using Contracts.Events;
using MediatR;
using Domain.Entities;
using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Helper;
using Application.Features.Users.DTOs;
using System.Linq.Expressions;

namespace Application.Features.Auth.Commands.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _irefreshTokenRepository;
    private readonly IMessageBus _bus;
    public RegisterUserCommandHandler(IUserRepository userRepository,
                                      IJwtTokenService jwtTokenService,
                                      IRefreshTokenRepository irefreshTokenRepository,
                                      IMessageBus bus)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _irefreshTokenRepository = irefreshTokenRepository;
        _bus = bus;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var response = new AuthResponse();
        var registerUser = request;
        var user = await _userRepository.GetByEmailAsync(registerUser.Email);
        if (user is not null)
        {
            response.Status = ResultStatus.Failed;
            response.ErrorMessage = "Email is already registered.";
            return response;
        }
        Expression<Func<User, bool>> condtion = (u => u.UserName.ToLower() == registerUser.UserName.ToLower());
        user = await _userRepository.GetItemByConditionAsync<User>(condtion);
        if (user is not null)
        {
            response.Status = ResultStatus.Failed;
            response.ErrorMessage = "Username is already taken.";
            return response;
        }

        // Hash the password with BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUser.Password);
        var newUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = registerUser.UserName,
            Email = registerUser.Email.ToLower(),
            PhoneNumber = registerUser.PhoneNumber,
            PasswordHash = passwordHash,
            Roles = new List<string> { "User" },
            Permissions = new List<string> { "ViewProduct" },
            CreatedAt = DateTime.UtcNow
        };

        await SetCurrentUserInfoAsync(newUser);
        await _userRepository.AddAsync<User>(newUser);

        var command = new SendEmailCommand()
        {
            ToMail = newUser.Email,
            Name = newUser.UserName,
            Subject = "Register Successfully in My School",
            Body = "<h1>Welcome to My School!</h1><p>Thank you for registering, " + newUser.UserName + ".</p>"
        };

        await _bus.PublishAsync(command);
        // Generate tokens
        var tokenResponse = await _jwtTokenService.GenerateTokenResponseAsync(newUser);

        // save refresh token
        await _irefreshTokenRepository.AddAsync(tokenResponse?.userRefreshTokenEntity!);

        response.Status = ResultStatus.Succeeded;
        response.AccessToken = tokenResponse?.AccessToken!;
        response.RefreshToken = tokenResponse!.RefreshToken;
        response.AccessTokenExpiry = tokenResponse.AccessTokenExpiry;
        response.RefreshTokenExpiry = tokenResponse.RefreshTokenExpiry;
        response.User = newUser.ToUserDtoResponse();
        return response;
    }

    private async Task SetCurrentUserInfoAsync(User newUser)
    {
        var currentUserContext = new CurrentUserContext
        {
            UserId = newUser.Id,
            Email = newUser.Email,
            Roles = newUser.Roles,
        };
        TellMe.SetCurrentUserContext(currentUserContext);
    }
}
