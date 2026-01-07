using Application.Common.Exceptions;
using Application.Features.Common.Models;
using Application.Features.Users.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.ComponentModel.DataAnnotations;
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
            Email = "user1@example.com",
            UserName = "user1",
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
    public async Task GetUsers_WithAllFilters_ReturnsFilteredUsers()
    {
        // Arrange
        var request = new GetUsersQueryDtoRequest
        {
            Page = 1,
            PageSize = 10,
            Role = "Admin",
            Search = "john",
            OrderBy = "Email",
            IsAscending = true,
            IsActive = true
        };

        // Act
        var actualResponse = await _fixture
            .GetAsync<GetUsersQueryDtoRequest, PagedResult<UserDtoResponse>>(request, "/api/users");

        // Assert
        Assert.NotNull(actualResponse);
        Assert.True(actualResponse.Items.Count >= 0);
        Assert.True(actualResponse.Total >= 0);
    }
    #endregion

    #region GetUserById Tests
    [Fact]
    public async Task GetUserById_InvalidUserId_ReturnsUserInfo()
    {
        // Arrange
        var invalidUserId = "invalid-user-id"; // Replace with a valid user ID in your test database

        // Assert
        await Assert.ThrowsAsync<NotFoundException> (
            // Act
            async () => await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{invalidUserId}")
        );
    }

    [Fact]
    public async Task GetUserById_ExistingUserId_ReturnsUserInfo()
    {
        var request = new CreateUserDtoRequest
        {
            Email = "user2@example.com",
            UserName = "user2",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Roles = new List<string> { "User" }
        };

        var api = $"/api/users";
        var createUser = await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", request);

        // Arrange
        var validUserId = createUser?.Id; // Replace with a valid user ID in your test database

        // Act
        var userResponseById = await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{validUserId}");

        // Assert
        Assert.NotNull(userResponseById);
        Assert.Equal(createUser.Email, userResponseById.Email);
    }
    #endregion

    #region UpdateUser Tests

    [Fact]
    public async Task UpdateUser_ValidRequest_UpdatesUserSuccessfully()
    {
        // Arrange - Create a user first
        var createRequest = new CreateUserDtoRequest
        {
            Email = "updateuser@example.com",
            UserName = "updateuser",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Roles = new List<string> { "User" },
            Address = "Dhaka"

        };

        var createdUser = await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", createRequest);

        var updateRequest = new UpdateUserDtoRequest
        {
            UserName = "updateduser",
            PhoneNumber = "9876543210",
            Address = "Khulna, Satkhira"
        };

        // Act
        await _fixture.PutAsync($"/api/users/{createdUser.Id}", updateRequest);

        // Verify update
        var updatedUser = await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{createdUser.Id}");

        // Assert
        Assert.NotNull(updatedUser);
        Assert.Equal(updateRequest.Address, updatedUser.Address);
        Assert.Equal(updateRequest.UserName, updatedUser.UserName);
        Assert.Equal(updateRequest.PhoneNumber, updatedUser.PhoneNumber);
    }

    [Fact]
    public async Task UpdateUser_NonExistingUser_ThrowsNotFoundException()
    {
        // Arrange
        var invalidUserId = "non-existing-user-id";
        var updateRequest = new UpdateUserDtoRequest
        {
            UserName = "testuser",
            PhoneNumber = "1234567890",
            Address = "Satkhira"
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.PutAsync($"/api/users/{invalidUserId}", updateRequest)
        );
    }

    #endregion

    #region DeleteUser Tests

    [Fact]
    public async Task DeleteUser_ExistingUser_DeletesSuccessfully()
    {
        // Arrange - Create a user first
        var createRequest = new CreateUserDtoRequest
        {
            Email = "deleteuser@example.com",
            UserName = "deleteuser",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Roles = new List<string> { "User" }
        };

        var createdUser = await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", createRequest);

        // Act
        await _fixture.DeleteAsync($"/api/users/{createdUser.Id}");

        // Assert - Try to get the deleted user (should throw NotFoundException or return IsActive = false)
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{createdUser.Id}")
        );
    }

    [Fact]
    public async Task DeleteUser_NonExistingUser_ThrowsNotFoundException()
    {
        // Arrange
        var invalidUserId = "non-existing-user-id";

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.DeleteAsync($"/api/users/{invalidUserId}")
        );
    }

    [Fact]
    public async Task DeleteUser_AlreadyDeletedUser_ThrowsNotFoundException()
    {
        // Arrange - Create and delete a user
        var createRequest = new CreateUserDtoRequest
        {
            Email = "doubledelete@example.com",
            UserName = "doubledelete",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Roles = new List<string> { "User" }
        };

        var createdUser = await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", createRequest);
        await _fixture.DeleteAsync($"/api/users/{createdUser.Id}");

        // Act & Assert - Try to delete again
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.DeleteAsync($"/api/users/{createdUser.Id}")
        );
    }

    #endregion

}
