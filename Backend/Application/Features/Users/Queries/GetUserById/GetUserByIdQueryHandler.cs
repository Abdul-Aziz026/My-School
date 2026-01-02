using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.Queries.GetUser;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserInfo>
{
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    public GetUserByIdQueryHandler(IUserRepository userRepository,
                                   ICacheService cacheService)
    {
        _userRepository = userRepository;
        _cacheService = cacheService ?? throw new Exception("CacheService NOT injected");
    }
    public async Task<UserInfo> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var email = request.Id;
        var cacheKey = $"UserInfo-{email}";
        if (await _cacheService.ExistsAsync(email))
        {
            return await _cacheService.GetAsync<UserInfo>(email);
        }
        var user = await _userRepository.GetByIdAsync<User>(email);
        if (user is null)
        {
            return null!;
        }
        var userInfo = new UserInfo
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
            ProfilePicture = user.ProfilePicture,
            Roles = user.Roles,
            CreatedAt = user.CreatedAt
        };
        await _cacheService.SetAsync<UserInfo>(cacheKey, userInfo, TimeSpan.FromMinutes(30));
        return userInfo;

    }
}
