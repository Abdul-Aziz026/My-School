using Application.Features.Common.Models;
using Application.Features.Users.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
            PhoneNumber = request.PhoneNumber
        };

        var api = $"/api/users";
        var actualResponse = await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", request);

        // assert
        Assert.Equal(expectedResponse, actualResponse);
    }
    #endregion

    #region GetUsers Tests
    [Fact]
    public async Task GetUsers_ValidRequest_ReturnsPagedUsers()
    {
        // Arrange
        var request = new GetUsersQueryDtoRequest
        {
            Page = 1,
            PageSize = 10,
            Role = "",
            Search = "",
            OrderBy = "",
            IsAscending = true,
            IsActive = true
        };

        // Convert DTO to query string
        var queryParams = new Dictionary<string, string?>
        {
            ["Page"] = request.Page.ToString(),
            ["PageSize"] = request.PageSize.ToString(),
            ["Role"] = request.Role,
            ["Search"] = request.Search,
            ["OrderBy"] = request.OrderBy,
            ["IsAscending"] = request.IsAscending.ToString(),
            ["IsActive"] = request.IsActive?.ToString()
        };

        var api = QueryHelpers.AddQueryString("/api/users", queryParams);
        //var api = "/api/users";

        // Act
        var actualResponse = await _fixture.GetAsync<PagedResult<UserDtoResponse>>(api);

        // Assert
        Assert.NotNull(actualResponse);
        Assert.True(actualResponse.Items.Count >= 0); // list can be empty
        Assert.True(actualResponse.Total >= 0);
    }
    #endregion
}
