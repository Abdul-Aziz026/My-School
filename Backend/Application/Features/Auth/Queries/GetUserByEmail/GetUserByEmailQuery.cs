using Application.Features.Auth.DTOs;
using Application.Features.Users.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Queries.GetUser;

public class GetUserByEmailQuery : IRequest<UserDtoResponse>
{
    public string Email { get; }
    public GetUserByEmailQuery(string email)
    {
        Email = email;
    }
}
