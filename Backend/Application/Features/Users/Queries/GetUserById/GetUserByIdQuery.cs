using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserInfo>
{
    public string Id { get; }
    public GetUserByIdQuery(string id)
    {
        Id = id;
    }
}
