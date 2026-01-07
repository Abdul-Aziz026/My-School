using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.Queries.GetUser;
using Application.Features.Users.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDtoResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    public GetUserByIdQueryHandler(IUserRepository userRepository,
                                   ICacheService cacheService)
    {
        _userRepository = userRepository;
        _cacheService = cacheService ?? throw new Exception("CacheService NOT injected");
    }
    public async Task<UserDtoResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var id = request.Id;
        var cacheKey = $"UserInfo-{id}";
        if (await _cacheService.ExistsAsync(cacheKey))
        {
            return await _cacheService.GetAsync<UserDtoResponse>(cacheKey);
        }
        var user = await _userRepository.GetByIdAsync<User>(id);
        if (user is null)
        {
            throw new NotFoundException("users not found");
        }
        var userDtoResponse = user.ToUserDtoResponse();
        await _cacheService.SetAsync<UserDtoResponse>(cacheKey, userDtoResponse, TimeSpan.FromMinutes(30));
        return userDtoResponse;
    }
}
