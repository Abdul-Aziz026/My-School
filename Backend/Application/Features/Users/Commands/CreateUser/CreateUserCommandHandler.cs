using Application.Features.Users.Commands.CreateUser;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if email already exists
            var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUserByEmail is not null)
                throw new Exception($"User with email {request.Email} already exists");

            // Hash the password with BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create user entity
            var newUser = new User
            {
                Email = request.Email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                ProfilePicture = request.ProfilePicture ?? string.Empty,
                Address = request.Address ?? string.Empty,
                Roles = request.Roles,
                Permissions = request.Permissions,
                FailedLoginAttempts = 0,
                LockoutEnabled = true
            };

            await _userRepository.AddAsync<User>(newUser);

            return new CreateUserResponse
            {
                Id = newUser.Id,
                Email = newUser.Email,
                UserName = newUser.UserName,
                Roles = newUser.Roles,
                IsActive = newUser.IsActive,
                CreatedAt = newUser.CreatedAt
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating user: {ex.Message}");
        }
    }
}
