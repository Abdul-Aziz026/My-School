using Application.Features.Auth.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands.RefreshToken;

public class RenewUserTokensCommand : IRequest<RefreshTokenResponse>
{
    public string RefreshToken { get; }
    public RenewUserTokensCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}
