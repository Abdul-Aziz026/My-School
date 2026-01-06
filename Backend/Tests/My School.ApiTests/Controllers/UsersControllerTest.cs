using Application.Features.Users.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using Tests.My_School.ApiTests.Fixtures;

namespace Tests.My_School.ApiTests.Controllers;

public class UsersControllerTest : IClassFixture<ApiTestFixture>
{
    private readonly ApiTestFixture _fixture;
    public UsersControllerTest(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region CreateUser Tests
    [Fact]
    public async Task CreateUser_ValidRequest_ReturnsCreatedUser()
    {
        // Arrange
        var request = new CreateUserDtoRequest
        {
            Email = "newuser@example.com",
            UserName = "newuser",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Roles = new List<string> { "User" }
        };

        var expectedResponse = new CreateUserDtoResponse
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            UserName = request.UserName,
            Roles = request.Roles,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var api = $"/api/users";
        var actualResponse = await _fixture.GetAsync<ActionResult<CreateUserDtoResponse>>(api);

        // assert
        Assert.Equal(expectedResponse, actualResponse);
    }
    #endregion
}
