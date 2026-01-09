
using Application.Common.Extensions;
using Application.Common.Interfaces.Repositories;
using Application.Features.Common.Models;
using Application.Features.Users.DTOs;
using Domain.Entities;
using MediatR;
using System.Linq.Expressions;

namespace Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDtoResponse>>
{
    private readonly IUserRepository _userRepository;
    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<PagedResult<UserDtoResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<User, bool>>? filter = x => true;
        if (request.IsActive.HasValue)
        {
            filter = filter.And(x => x.IsActive == request.IsActive.Value);
        }
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            filter = filter.And(x => x.Roles.Contains(request.Role));
        }
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            filter = filter.And(x => x.UserName.Contains(request.Search) || x.Email.Contains(request.Search) || x.PhoneNumber.Contains(request.Search));
        }

        var totalCount = await _userRepository.CountAsync(filter);
        var users = await _userRepository.GetPagedAsync(filter: filter, 
                            pageNumber: request.Page,
                            pageSize: request.PageSize,
                            orderBy: GetOrderByExpression(request.OrderBy), 
                            ascending: request.IsAscending);

        var userDtos = users.Select(x => x.ToUserDtoResponse()).ToList();

        return new PagedResult<UserDtoResponse>
        {
            Items = userDtos,
            Total = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private Expression<Func<User, object>>? GetOrderByExpression(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return x => x.CreatedAt; // Default ordering

        return orderBy.ToLower() switch
        {
            "username" => x => x.UserName,
            "email" => x => x.Email,
            "createdat" => x => x.CreatedAt!,
            "updatedat" => x => x.UpdatedAt!,
            _ => x => x.CreatedAt
        };
    }
}
