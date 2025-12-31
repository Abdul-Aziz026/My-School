using Application.Features.Auth.DTOs;
using Application.Helper;
using Application.Interfaces.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var updatedUser = request.UpdateUserProfileDto;
        var currentUserId = request.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
        var user = await _userRepository.GetByIdAsync<User>(currentUserId) ?? throw new UnauthorizedAccessException("User not found.");
        await ApplyProfileUpdatesToUser(updatedUser, user);
        await _userRepository.UpdateAsync<User>(user);
        return;
    }

    private static async Task ApplyProfileUpdatesToUser(UpdateUserProfileDto updatedUser, User user)
    {
        if (!string.IsNullOrWhiteSpace(updatedUser.UserName))
        {
            user.Email = updatedUser.UserName;
        }
        if (!string.IsNullOrWhiteSpace(updatedUser.PhoneNumber))
        {
            user.Email = updatedUser.PhoneNumber;
        }
        if (!string.IsNullOrWhiteSpace(updatedUser.ProfilePicture))
        {
            user.Email = updatedUser.ProfilePicture;
        }
        if (!string.IsNullOrWhiteSpace(updatedUser.Address))
        {
            user.Email = updatedUser.Address;
        }
    }
}
