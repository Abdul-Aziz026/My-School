using Application.Common.Exceptions;
using Application.Features.Common.Models;
using Application.Features.Users.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
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

        // Act
        var actualResponse = await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", request);

        // Assert
        Assert.NotNull(actualResponse);
        Assert.NotNull(actualResponse.Id);
        Assert.Equal(request.Email, actualResponse.Email);
        Assert.Equal(request.UserName, actualResponse.UserName);
        Assert.Equal(request.PhoneNumber, actualResponse.PhoneNumber);
        Assert.Equal(request.Roles, actualResponse.Roles);
        Assert.True(actualResponse.IsActive);
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ThrowsValidationException()
    {
        // Arrange - Create first user
        var request = new CreateUserDtoRequest
        {
            Email = "duplicate@example.com",
            UserName = "user1",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Roles = new List<string> { "User" }
        };

        await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", request);

        // Try to create another user with same email
        var duplicateRequest = new CreateUserDtoRequest
        {
            Email = "duplicate@example.com",
            UserName = "user2",
            Password = "Password123!",
            PhoneNumber = "9876543210",
            Roles = new List<string> { "User" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", duplicateRequest)
        );
    }

    [Fact]
    public async Task CreateUser_InvalidEmail_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateUserDtoRequest
        {
            Email = "invalid-email",
            UserName = "testuser",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Roles = new List<string> { "User" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", request)
        );
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
        Assert.True(actualResponse.Page == 1);
        Assert.True(actualResponse.PageSize == 10);
    }

    [Fact]
    public async Task GetUsers_WithPagination_ReturnsCorrectPage()
    {
        // Arrange - Create multiple users
        for (int i = 1; i <= 5; i++)
        {
            var createRequest = new CreateUserDtoRequest
            {
                Email = $"pageuser{i}@example.com",
                UserName = $"pageuser{i}",
                Password = "Password123!",
                PhoneNumber = $"123456789{i}",
                Roles = new List<string> { "User" }
            };
            await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", createRequest);
        }

        var request = new GetUsersQueryDtoRequest
        {
            Page = 1,
            PageSize = 3
        };

        // Act
        var response = await _fixture
            .GetAsync<GetUsersQueryDtoRequest, PagedResult<UserDtoResponse>>(request, "/api/users");

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Items.Count <= 3);
        Assert.True(response.Total >= 5);
    }

    [Fact]
    public async Task GetUsers_WithSearchFilter_ReturnsMatchingUsers()
    {
        // Arrange - Create a user with specific name
        var createRequest = new CreateUserDtoRequest
        {
            Email = "searchable@example.com",
            UserName = "uniquesearchname",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Roles = new List<string> { "User" }
        };
        await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", createRequest);

        var request = new GetUsersQueryDtoRequest
        {
            Page = 1,
            PageSize = 10,
            Search = "uniquesearchname"
        };

        // Act
        var response = await _fixture
            .GetAsync<GetUsersQueryDtoRequest, PagedResult<UserDtoResponse>>(request, "/api/users");

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Items.Count > 0);
        Assert.Contains(response.Items, u => u.UserName.Contains("uniquesearchname"));
    }

    #endregion

    #region GetUserById Tests

    [Fact]
    public async Task GetUserById_InvalidUserId_ThrowsNotFoundException()
    {
        // Arrange
        var invalidUserId = "invalid-user-id-123456";

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{invalidUserId}")
        );
    }

    [Fact]
    public async Task GetUserById_ExistingUserId_ReturnsUserInfo()
    {
        // Arrange - Create user
        var createRequest = new CreateUserDtoRequest
        {
            Email = "getbyid@example.com",
            UserName = "getbyiduser",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Roles = new List<string> { "User" }
        };

        var createdUser = await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", createRequest);

        // Act
        var userResponse = await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{createdUser.Id}");

        // Assert
        Assert.NotNull(userResponse);
        Assert.Equal(createdUser.Id, userResponse.Id);
        Assert.Equal(createdUser.Email, userResponse.Email);
        Assert.Equal(createdUser.UserName, userResponse.UserName);
        Assert.Equal(createdUser.PhoneNumber, userResponse.PhoneNumber);
    }

    [Fact]
    public async Task GetUserById_DeletedUser_ThrowsNotFoundException()
    {
        // Arrange - Create and delete user
        var createRequest = new CreateUserDtoRequest
        {
            Email = "deleted@example.com",
            UserName = "deleteduser",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Roles = new List<string> { "User" }
        };

        var createdUser = await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", createRequest);
        await _fixture.DeleteAsync($"/api/users/{createdUser.Id}");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{createdUser.Id}")
        );
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

        // Verify user was created with original values
        var userBeforeUpdate = await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{createdUser.Id}");
        Assert.Equal("updateuser", userBeforeUpdate.UserName);
        Assert.Equal("1234567890", userBeforeUpdate.PhoneNumber);
        Assert.Equal("Dhaka", userBeforeUpdate.Address);

        var updateRequest = new UpdateUserDtoRequest
        {
            UserName = "updateduser",
            PhoneNumber = "9876543210",
            Address = "Khulna, Satkhira"
        };

        // Act - Update the user
        await _fixture.PutAsync($"/api/users/{createdUser.Id}", updateRequest);
        await Task.Delay(1000);
        // Verify update by fetching fresh from database
        var updatedUser = await _fixture.GetAsync<UserDtoResponse>($"/api/users/{createdUser.Id}");

        // Assert
        Assert.NotNull(updatedUser);
        Assert.Equal(updateRequest.UserName, updatedUser.UserName);
        Assert.Equal(updateRequest.PhoneNumber, updatedUser.PhoneNumber);
        Assert.Equal(updateRequest.Address, updatedUser.Address);

        // Ensure other fields remain unchanged
        Assert.Equal(createdUser.Email, updatedUser.Email);
        Assert.Equal(createdUser.Id, updatedUser.Id);
    }

    [Fact]
    public async Task UpdateUser_NonExistingUser_ThrowsNotFoundException()
    {
        // Arrange
        var invalidUserId = "non-existing-user-id-789";
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

    [Fact]
    public async Task UpdateUser_PartialUpdate_UpdatesOnlyProvidedFields()
    {
        // Arrange - Create user
        var createRequest = new CreateUserDtoRequest
        {
            Email = "partial@example.com",
            UserName = "partialuser",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Address = "Original Address",
            Roles = new List<string> { "User" }
        };

        var createdUser = await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", createRequest);

        // Update only phone number
        var updateRequest = new UpdateUserDtoRequest
        {
            PhoneNumber = "9999999999"
            // UserName and Address not provided
        };

        // Act
        await _fixture.PutAsync($"/api/users/{createdUser.Id}", updateRequest);

        var updatedUser = await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{createdUser.Id}");

        // Assert
        Assert.Equal("9999999999", updatedUser.PhoneNumber);
        Assert.Equal("partialuser", updatedUser.UserName); // Should remain unchanged
        Assert.Equal("Original Address", updatedUser.Address); // Should remain unchanged
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

        // Assert - Try to get the deleted user (should throw NotFoundException)
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{createdUser.Id}")
        );
    }

    [Fact]
    public async Task DeleteUser_NonExistingUser_ThrowsNotFoundException()
    {
        // Arrange
        var invalidUserId = "non-existing-user-id-456";

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

    #region Integration Tests

    [Fact]
    public async Task UserLifecycle_CreateUpdateDelete_WorksCorrectly()
    {
        // Create
        var createRequest = new CreateUserDtoRequest
        {
            Email = "lifecycle@example.com",
            UserName = "lifecycleuser",
            Password = "Password123!",
            PhoneNumber = "1111111111",
            Address = "Initial Address",
            Roles = new List<string> { "User" }
        };

        var createdUser = await _fixture.PostAsync<CreateUserDtoRequest, CreateUserDtoResponse>("/api/users", createRequest);
        Assert.NotNull(createdUser);
        Assert.Equal("lifecycleuser", createdUser.UserName);

        // Update
        var updateRequest = new UpdateUserDtoRequest
        {
            UserName = "updatedlifecycle",
            PhoneNumber = "2222222222",
            Address = "Updated Address"
        };

        await _fixture.PutAsync($"/api/users/{createdUser.Id}", updateRequest);

        var updatedUser = await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{createdUser.Id}");
        Assert.Equal("updatedlifecycle", updatedUser.UserName);
        Assert.Equal("2222222222", updatedUser.PhoneNumber);

        // Delete
        await _fixture.DeleteAsync($"/api/users/{createdUser.Id}");

        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.GetAsync<string, UserDtoResponse>(null!, $"/api/users/{createdUser.Id}")
        );
    }

    #endregion
}