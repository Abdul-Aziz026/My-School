
using Application.Features.Auth.DTOs;
using Application.Helper;
using Application.Interfaces.Publisher;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Contracts.Events;
using MediatR;
using Domain.Entities;

namespace Application.Features.Auth.Commands.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RefreshTokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMessageBus _bus;
    public RegisterUserCommandHandler(IUserRepository userRepository,
                                      IJwtTokenService jwtTokenService,
                                      IMessageBus bus)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _bus = bus;
    }

    public async Task<RefreshTokenResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var response = new RefreshTokenResponse();
        var registerUser = request.RegisterDto;
        var user = await _userRepository.GetByEmailAsync(registerUser.Email);
        if (user is not null)
        {
            response.Result = ActionEvent.Failed;
            return response;
        }
        // Hash the password with BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUser.Password);
        var newUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = registerUser.UserName,
            Email = registerUser.Email,
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
        return await _jwtTokenService.GenerateTokenResponseAsync(newUser);
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
