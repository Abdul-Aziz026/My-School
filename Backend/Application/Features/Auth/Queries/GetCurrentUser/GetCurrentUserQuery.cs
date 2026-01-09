using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<CurrentUserDtoResponse>;
