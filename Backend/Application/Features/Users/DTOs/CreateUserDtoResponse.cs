using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Users.DTOs;

public class CreateUserDtoResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        if (obj is not CreateUserDtoResponse)
        {
            return false;
        }
        var createdUser = (CreateUserDtoResponse)obj;
        return this.Email == createdUser.Email &&
               this.UserName == createdUser.UserName &&
               this.IsActive == createdUser.IsActive &&
               this.PhoneNumber == createdUser.PhoneNumber;
    }
}

public static class CreateUserDtoResponseExtensions
{
    public static CreateUserDtoResponse ToCreateUserDtoResponse(this User newUser)
    {
        return new CreateUserDtoResponse
        {
            Id = newUser.Id,
            Email = newUser.Email,
            UserName = newUser.UserName,
            Roles = newUser.Roles,
            IsActive = newUser.IsActive,
            CreatedAt = newUser.CreatedAt,
            PhoneNumber = newUser.PhoneNumber
        };
    }
}
