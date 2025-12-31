
using Application.Features.Common.Models;
using MediatR;

namespace Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    public Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var condition = true;
        // implement the logic to get users based on the request parameters
        // return a PagedResult<UserDto> containing the users

        throw new NotImplementedException();
    }
}
