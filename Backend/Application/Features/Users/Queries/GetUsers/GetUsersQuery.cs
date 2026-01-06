
using Application.Features.Common.Models;
using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<PagedResult<UserDtoResponse>>
{
    public string Role { get; set; } = string.Empty;
    public string Search { get; set; } = string.Empty;
    public string OrderBy { get; set; } = string.Empty;
    public bool IsAscending { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
