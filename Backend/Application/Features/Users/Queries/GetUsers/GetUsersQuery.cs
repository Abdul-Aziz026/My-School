
using Application.Features.Common.Models;
using MediatR;

namespace Application.Features.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<PagedResult<UserDto>>
{
    public string Role { get; set; } = string.Empty;
    public string Search { get; set; } = string.Empty;
    public string OrderBy { get; set; } = string.Empty;
    public bool IsAscending { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}