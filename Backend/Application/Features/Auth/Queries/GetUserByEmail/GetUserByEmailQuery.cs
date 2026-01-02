using Application.Features.Auth.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Queries.GetUser;

public class GetUserByEmailQuery : IRequest<UserInfo>
{
    public string Email { get; }
    public GetUserByEmailQuery(string email)
    {
        Email = email;
    }
}
