using Domain.Entities;

namespace Application.Features.Users.DTOs;

public class UserDtoResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public static class UserDtoExtensions
{
    public static UserDtoResponse ToUserDtoResponse(this User user)
    {
        return new UserDtoResponse
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            Roles = user.Roles,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Address = user.Address
        };
    }
}
