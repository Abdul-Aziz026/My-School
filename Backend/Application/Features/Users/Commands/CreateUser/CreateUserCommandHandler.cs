using Application.Common.Interfaces.Repositories;
using Application.Common.Validator;
using Application.Features.Users.Commands.CreateUser;
using Application.Features.Users.DTOs;
using Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserDtoResponse>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<CreateUserDtoResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        EmailValidator.Validate(request.Email);
        // Check if email already exists
        var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUserByEmail is not null)
            throw new ArgumentException($"User with email {request.Email} already exists");

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

        try
        {
            await _userRepository.AddAsync<User>(newUser);

            return newUser.ToCreateUserDtoResponse();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating user: {ex.Message}");
        }
    }
}
