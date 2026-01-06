using Application.Features.Users.Queries.GetUsers;
using System;

namespace Application.Features.Users.DTOs;

public class GetUsersQueryDtoRequest
{
    public string Role { get; set; } = string.Empty;
    public string Search { get; set; } = string.Empty;
    public string OrderBy { get; set; } = string.Empty;
    public bool IsAscending { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetUsersQuery ToGetUsersQuery()
    {
        return new GetUsersQuery
        {
            Role = this.Role,
            Search = this.Search,
            OrderBy = this.OrderBy,
            IsAscending = this.IsAscending,
            IsActive = this.IsActive,
            Page = this.Page,
            PageSize = this.PageSize
        };
    }
}
