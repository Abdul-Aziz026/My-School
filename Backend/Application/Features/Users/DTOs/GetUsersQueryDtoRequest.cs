using System;
using Application.Features.Users.Queries.GetUsers;

namespace Application.Features.Users.DTOs;

public class GetUsersQueryDtoRequest
{
    public string? Role { get; set; }
    public string? Search { get; set; }
    public string? OrderBy { get; set; }
    public bool IsAscending { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetUsersQuery ToGetUsersQuery()
    {
        return new GetUsersQuery
        {
            Role = this.Role!,
            Search = this.Search!,
            OrderBy = this.OrderBy!,
            IsAscending = this.IsAscending,
            IsActive = this.IsActive,
            Page = this.Page,
            PageSize = this.PageSize
        };
    }
}
