using Application.Features.Auth.DTOs;
using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserDtoResponse>
{
    public string Id { get; }
    public GetUserByIdQuery(string id)
    {
        Id = id;
    }
}
