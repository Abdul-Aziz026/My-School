using Application.Features.Users.DTOs;
using System;

namespace Application.Features.Auth.DTOs;

public class CurrentUserDtoResponse
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public UserDtoResponse? User { get; set; }
}
