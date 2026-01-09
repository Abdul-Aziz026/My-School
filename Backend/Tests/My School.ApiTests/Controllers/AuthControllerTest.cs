using Application.Common.Exceptions;
using Application.Features.Auth.DTOs;
using Tests.My_School.ApiTests.Fixtures;

namespace Tests.My_School.ApiTests.Controllers;

public class AuthControllerTests : IClassFixture<ApiTestFixture>
{
    private readonly ApiTestFixture _fixture;

    public AuthControllerTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Register Tests

    [Fact]
    public async Task Register_ValidRequest_ReturnsAuthResponseWithToken()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "newuser@example.com",
            UserName = "newuser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        // Act
        var response = await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(ResultStatus.Succeeded, response.Status);
        Assert.NotNull(response.AccessToken);
        Assert.NotNull(response.User);
        Assert.Equal(registerDto.Email, response.User.Email);
        Assert.Equal(registerDto.UserName, response.User.UserName);
        Assert.True(response.AccessTokenExpiry > DateTime.UtcNow);
        Assert.NotNull(response.RefreshToken);
        Assert.True(response.RefreshTokenExpiry > DateTime.UtcNow);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ThrowsArgumentException()
    {
        // Arrange - First registration
        var registerDto = new RegisterDto
        {
            Email = "duplicate@example.com",
            UserName = "user1",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // Try to register with same email but different username
        var duplicateDto = new RegisterDto
        {
            Email = "duplicate@example.com",
            UserName = "user2",
            Password = "Password123!",
            PhoneNumber = "9876543210"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", duplicateDto)
        );
    }

    [Fact]
    public async Task Register_DuplicateUserName_ThrowsArgumentException()
    {
        // Arrange - First registration
        var registerDto = new RegisterDto
        {
            Email = "user1@example.com",
            UserName = "duplicateusername",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // Try to register with same username but different email
        var duplicateDto = new RegisterDto
        {
            Email = "user2@example.com",
            UserName = "duplicateusername",
            Password = "Password123!",
            PhoneNumber = "9876543210"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", duplicateDto)
        );
    }

    [Fact]
    public async Task Register_InvalidEmail_ThrowsArgumentException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "invalid-email",
            UserName = "testuser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto)
        );
    }

    [Fact]
    public async Task Register_MissingEmail_ThrowsArgumentException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "",
            UserName = "testuser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto)
        );
    }

    [Fact]
    public async Task Register_MissingUserName_ThrowsArgumentException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "testuser@example.com",
            UserName = "",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto)
        );
    }

    [Fact]
    public async Task Register_MissingPassword_ThrowsArgumentException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "testuser@example.com",
            UserName = "testuser",
            Password = "",
            PhoneNumber = "1234567890"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto)
        );
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_ValidCredentials_ReturnsAuthResponseWithToken()
    {
        // Arrange - First register a user
        var registerDto = new RegisterDto
        {
            Email = "loginuser@example.com",
            UserName = "loginuser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        var loginDto = new LoginDto
        {
            Email = "loginuser@example.com",
            Password = "Password123!"
        };

        // Act
        var response = await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(ResultStatus.Succeeded, response.Status);
        Assert.NotNull(response.AccessToken);
        Assert.NotNull(response.User);
        Assert.Equal(loginDto.Email, response.User.Email);
        Assert.True(response.AccessTokenExpiry > DateTime.UtcNow);
        Assert.NotNull(response.RefreshToken);
        Assert.True(response.RefreshTokenExpiry > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_InvalidEmail_ThrowsUnauthorizedException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto)
        );
    }

    [Fact]
    public async Task Login_InvalidPassword_ThrowsUnauthorizedException()
    {
        // Arrange - Register a user
        var registerDto = new RegisterDto
        {
            Email = "wrongpassword@example.com",
            UserName = "wrongpassworduser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        var loginDto = new LoginDto
        {
            Email = "wrongpassword@example.com",
            Password = "WrongPassword123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto)
        );
    }

    [Fact]
    public async Task Login_EmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "",
            Password = "Password123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto)
        );
    }

    [Fact]
    public async Task Login_EmptyPassword_ThrowsArgumentException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto)
        );
    }

    [Fact]
    public async Task Login_CaseSensitiveEmail_LoginSucceeds()
    {
        // Arrange - Register with lowercase
        var registerDto = new RegisterDto
        {
            Email = "casetest@example.com",
            UserName = "casetest",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // Try login with different case
        var loginDto = new LoginDto
        {
            Email = "CaseTest@Example.Com",
            Password = "Password123!"
        };

        // Act
        var response = await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto);

        // Assert - Should succeed (emails are typically case-insensitive)
        Assert.NotNull(response);
        Assert.Equal(ResultStatus.Succeeded, response.Status);
    }

    #endregion

    #region Refresh Token Tests

    [Fact]
    public async Task Refresh_ValidRefreshToken_ReturnsNewTokens()
    {
        // Arrange - Register and login to get refresh token in cookie
        var registerDto = new RegisterDto
        {
            Email = "refreshuser@example.com",
            UserName = "refreshuser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        var loginResponse = await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // The refresh token should be set in cookies automatically by the controller

        // Act
        var response = await _fixture.PostAsync<AuthResponse>("/api/auth/refresh");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(ResultStatus.Succeeded, response.Status);
        Assert.NotNull(response.AccessToken);
        Assert.NotNull(response.User);
        Assert.True(response.AccessTokenExpiry > DateTime.UtcNow);
        Assert.NotNull(response.RefreshToken);
        // New token should be different from original
        Assert.NotEqual(loginResponse.AccessToken, response.AccessToken);
    }

    [Fact]
    public async Task Refresh_MissingRefreshToken_ThrowsUnauthorizedException()
    {
        // Arrange - Clear any existing cookies
        // Note: You may need to implement ClearCookies() in your fixture
        _fixture.ClearCookies();
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            
            async () => await _fixture.PostAsync<AuthResponse>("/api/auth/refresh")
        );
    }

    [Fact]
    public async Task Refresh_InvalidRefreshToken_ThrowsUnauthorizedException()
    {
        // Arrange - Set an invalid refresh token in cookies
        // Note: You may need to implement SetCookie() in your fixture
        _fixture.SetCookie("refreshToken", "invalid-token-12345");

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _fixture.PostAsync<AuthResponse>("/api/auth/refresh")
        );
    }

    [Fact]
    public async Task Refresh_AfterLogout_ThrowsUnauthorizedException()
    {
        // Arrange - Register, login, then logout
        var registerDto = new RegisterDto
        {
            Email = "refreshafterlogout@example.com",
            UserName = "refreshafterlogout",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // Logout (should invalidate refresh token)
        await _fixture.PostAsync<object>("/api/auth/logout");

        // Act & Assert - Try to refresh after logout
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _fixture.PostAsync<AuthResponse>("/api/auth/refresh")
        );
    }

    [Fact]
    public async Task Refresh_MultipleTimes_EachReturnsNewToken()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "multirefresh@example.com",
            UserName = "multirefresh",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        var initialResponse = await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // Act - Refresh multiple times
        var refresh1 = await _fixture.PostAsync<AuthResponse>("/api/auth/refresh");

        var refresh2 = await _fixture.PostAsync<AuthResponse>("/api/auth/refresh");

        var refresh3 = await _fixture.PostAsync<AuthResponse>("/api/auth/refresh");

        // Assert - All tokens should be different
        Assert.NotEqual(initialResponse?.AccessToken, refresh1?.AccessToken);
        Assert.NotEqual(refresh1?.AccessToken, refresh2?.AccessToken);
        Assert.NotEqual(refresh2?.AccessToken, refresh3?.AccessToken);

        // All should have succeeded status
        Assert.Equal(ResultStatus.Succeeded, refresh1?.Status);
        Assert.Equal(ResultStatus.Succeeded, refresh2?.Status);
        Assert.Equal(ResultStatus.Succeeded, refresh3?.Status);
    }

    #endregion

    #region Logout Tests

    [Fact]
    public async Task Logout_ValidSession_LogsOutSuccessfully()
    {
        // Arrange - Register and login
        var registerDto = new RegisterDto
        {
            Email = "logoutuser@example.com",
            UserName = "logoutuser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // Act
        var response = await _fixture.PostAsync<object>("/api/auth/logout");

        // Assert
        Assert.NotNull(response);

        // Verify that refresh token is invalidated
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _fixture.PostAsync<AuthResponse>("/api/auth/refresh")
        );
    }

    [Fact]
    public async Task Logout_WithoutSession_StillSucceeds()
    {
        // Arrange - No active session

        // Act - Should not throw exception
        var response = await _fixture.PostAsync<object>("/api/auth/logout");

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task Logout_TwiceInRow_BothSucceed()
    {
        // Arrange - Register
        var registerDto = new RegisterDto
        {
            Email = "doublelogout@example.com",
            UserName = "doublelogout",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // Act - Logout twice
        var response1 = await _fixture.PostAsync<object>("/api/auth/logout");
        var response2 = await _fixture.PostAsync<object>("/api/auth/logout");

        // Assert - Both should succeed
        Assert.NotNull(response1);
        Assert.NotNull(response2);
    }

    #endregion

    #region Me (Current User) Tests

    [Fact]
    public async Task Me_AuthenticatedUser_ReturnsUserInfo()
    {
        // Arrange - Register and set auth token
        var registerDto = new RegisterDto
        {
            Email = "meuser@example.com",
            UserName = "meuser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        var loginResponse = await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);
        _fixture.SetAuthToken(loginResponse.AccessToken);

        // Act
        var currentUserResponse = await _fixture.GetAsync<object, CurrentUserDtoResponse>(null!, "/api/auth/me");

        // Assert
        Assert.NotNull(currentUserResponse);
        Assert.Equal(registerDto.Email, currentUserResponse?.User?.Email);
        Assert.Equal(registerDto.UserName, currentUserResponse?.User?.UserName);

        // Cleanup
        _fixture.ClearAuthToken();
    }

    [Fact]
    public async Task Me_UnauthenticatedUser_ThrowsNotFoundException()
    {
        // Arrange - No auth token set
        _fixture.ClearAuthToken();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.GetAsync<object, UserInfo>(null!, "/api/auth/me")
        );
    }

    [Fact]
    public async Task Me_InvalidToken_ThrowsNotFoundException()
    {
        // Arrange - Set invalid token
        _fixture.SetAuthToken("invalid-token-xyz");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.GetAsync<object, UserInfo>(null!, "/api/auth/me")
        );

        // Cleanup
        _fixture.ClearAuthToken();
    }

    [Fact]
    public async Task Me_ExpiredToken_ThrowsNotFoundException()
    {
        // Arrange - Register user, wait for token to expire
        // Note: In real tests, you might need to mock token expiry
        // or generate an expired token manually

        var registerDto = new RegisterDto
        {
            Email = "expiredtoken@example.com",
            UserName = "expiredtoken",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        var loginResponse = await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // Set an expired token (you may need to generate this separately)
        _fixture.SetAuthToken("expired.jwt.token");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.GetAsync<object, UserInfo>(null!, "/api/auth/me")
        );

        // Cleanup
        _fixture.ClearAuthToken();
    }

    [Fact]
    public async Task Me_AfterTokenRefresh_ReturnsUpdatedUserResponse()
    {
        // Arrange - Register and get initial token
        var registerDto = new RegisterDto
        {
            Email = "refreshme@example.com",
            UserName = "refreshme",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        var initialResponse = await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // Refresh token
        var refreshResponse = await _fixture.PostAsync<AuthResponse>("/api/auth/refresh");

        // Set new token
        _fixture.SetAuthToken(refreshResponse.AccessToken);

        // Act
        var currentLoginUser = await _fixture.GetAsync<object, CurrentUserDtoResponse>(null!, "/api/auth/me");

        // Assert
        Assert.NotNull(currentLoginUser);
        Assert.Equal(registerDto.Email, currentLoginUser?.User?.Email);
        Assert.Equal(registerDto.UserName, currentLoginUser?.User?.UserName);

        // Cleanup
        _fixture.ClearAuthToken();
    }

    #endregion

    #region Forgot Password Tests

    [Fact]
    public async Task ForgotPassword_ValidEmail_ReturnsSuccessMessage()
    {
        // Arrange - Register a user first
        var registerDto = new RegisterDto
        {
            Email = "forgotpassword@example.com",
            UserName = "forgotuser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        var forgotPasswordDto = new ForgotPasswordDto
        {
            Email = "forgotpassword@example.com"
        };

        // Act
        var response = await _fixture.PostAsync<ForgotPasswordDto, object>("/api/auth/forgot-password", forgotPasswordDto);

        // Assert
        Assert.NotNull(response);
        // The endpoint returns success message regardless (security best practice)
    }

    [Fact]
    public async Task ForgotPassword_NonExistentEmail_ReturnsSuccessMessage()
    {
        // Arrange
        var forgotPasswordDto = new ForgotPasswordDto
        {
            Email = "nonexistent@example.com"
        };

        // Act
        var response = await _fixture.PostAsync<ForgotPasswordDto, object>("/api/auth/forgot-password", forgotPasswordDto);

        // Assert
        Assert.NotNull(response);
        // Returns same success message to prevent email enumeration
    }

    [Fact]
    public async Task ForgotPassword_InvalidEmailFormat_ThrowsArgumentException()
    {
        // Arrange
        var forgotPasswordDto = new ForgotPasswordDto
        {
            Email = "invalid-email-format"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<ForgotPasswordDto, object>("/api/auth/forgot-password", forgotPasswordDto)
        );
    }

    [Fact]
    public async Task ForgotPassword_EmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        var forgotPasswordDto = new ForgotPasswordDto
        {
            Email = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<ForgotPasswordDto, object>("/api/auth/forgot-password", forgotPasswordDto)
        );
    }

    [Fact]
    public async Task ForgotPassword_NullEmail_ThrowsArgumentException()
    {
        // Arrange
        var forgotPasswordDto = new ForgotPasswordDto
        {
            Email = null!
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<ForgotPasswordDto, object>("/api/auth/forgot-password", forgotPasswordDto)
        );
    }

    #endregion

    #region Reset Password Tests

    [Fact]
    public async Task ResetPassword_ValidTokenAndPassword_ResetsSuccessfully()
    {
        // Arrange - Register user and initiate forgot password
        var registerDto = new RegisterDto
        {
            Email = "resetpassword@example.com",
            UserName = "resetuser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        var forgotPasswordDto = new ForgotPasswordDto
        {
            Email = "resetpassword@example.com"
        };

        await _fixture.PostAsync<ForgotPasswordDto, object>("/api/auth/forgot-password", forgotPasswordDto);

        // In real scenario, you'd need to extract the reset token from the email
        // For testing, you might need to retrieve it from database or mock it
        var resetPasswordDto = new ResetPasswordDto
        {
            Email = "resetpassword@example.com",
            Token = "abcdefghijklmnopqrstuvwxyz", // This should be retrieved from email/database
            NewPassword = "NewPassword123!"
        };

        // Act
        var response = await _fixture.PostAsync<ResetPasswordDto, object>("/api/auth/reset-password", resetPasswordDto);

        // Assert
        Assert.NotNull(response);

        // Verify can login with new password
        var loginDto = new LoginDto
        {
            Email = "resetpassword@example.com",
            Password = "NewPassword123!"
        };

        var loginResponse = await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto);
        Assert.NotNull(loginResponse);
        Assert.Equal(ResultStatus.Succeeded, loginResponse.Status);
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_ThrowsArgumentException()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDto
        {
            Email = "test@example.com",
            Token = "invalid-token-xyz",
            NewPassword = "NewPassword123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<ResetPasswordDto, object>("/api/auth/reset-password", resetPasswordDto)
        );
    }

    [Fact]
    public async Task ResetPassword_ExpiredToken_ThrowsArgumentException()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDto
        {
            Email = "test@example.com",
            Token = "expired-token-abc",
            NewPassword = "NewPassword123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<ResetPasswordDto, object>("/api/auth/reset-password", resetPasswordDto)
        );
    }

    [Fact]
    public async Task ResetPassword_WeakNewPassword_ThrowsArgumentException()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDto
        {
            Email = "test@example.com",
            Token = "valid-token",
            NewPassword = "weak" // Too weak
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<ResetPasswordDto, object>("/api/auth/reset-password", resetPasswordDto)
        );
    }

    [Fact]
    public async Task ResetPassword_EmptyNewPassword_ThrowsArgumentException()
    {
        // Arrange
        var resetPasswordDto = new ResetPasswordDto
        {
            Email = "test@example.com",
            Token = "valid-token",
            NewPassword = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<ResetPasswordDto, object>("/api/auth/reset-password", resetPasswordDto)
        );
    }

    [Fact]
    public async Task ResetPassword_MismatchedEmail_ThrowsArgumentException()
    {
        // Arrange - User requests reset for one email but provides different email in reset
        var forgotPasswordDto = new ForgotPasswordDto
        {
            Email = "original@example.com"
        };

        await _fixture.PostAsync<ForgotPasswordDto, object>("/api/auth/forgot-password", forgotPasswordDto);

        var resetPasswordDto = new ResetPasswordDto
        {
            Email = "different@example.com", // Different email
            Token = "valid-token",
            NewPassword = "NewPassword123!"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _fixture.PostAsync<ResetPasswordDto, object>("/api/auth/reset-password", resetPasswordDto)
        );
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task AuthFlow_RegisterLoginRefreshLogout_WorksCorrectly()
    {
        // Register
        var registerDto = new RegisterDto
        {
            Email = "authflow@example.com",
            UserName = "authflowuser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        var registerResponse = await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);
        Assert.NotNull(registerResponse);
        Assert.Equal(ResultStatus.Succeeded, registerResponse.Status);
        Assert.NotNull(registerResponse.AccessToken);
        Assert.NotNull(registerResponse.RefreshToken);

        // Login
        var loginDto = new LoginDto
        {
            Email = "authflow@example.com",
            Password = "Password123!"
        };

        var loginResponse = await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto);
        Assert.NotNull(loginResponse);
        Assert.Equal(ResultStatus.Succeeded, loginResponse.Status);
        Assert.NotNull(loginResponse.AccessToken);

        // Access profile with token
        _fixture.SetAuthToken(loginResponse.AccessToken);
        var currentUserDtoResponse = await _fixture.GetAsync<object?, CurrentUserDtoResponse>(null!, "/api/auth/me");
        Assert.NotNull(currentUserDtoResponse);
        Assert.Equal("authflow@example.com", currentUserDtoResponse?.User?.Email);

        // Refresh token
        var refreshResponse = await _fixture.PostAsync<AuthResponse>("/api/auth/refresh");
        Assert.NotNull(refreshResponse);
        Assert.Equal(ResultStatus.Succeeded, refreshResponse.Status);
        Assert.NotNull(refreshResponse.AccessToken);

        // Logout
        await _fixture.PostAsync<object>("/api/auth/logout");

        // Verify refresh fails after logout
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _fixture.PostAsync<AuthResponse>("/api/auth/refresh")
        );

        // Cleanup
        _fixture.ClearAuthToken();
    }


    //[Fact]
    public async Task AuthFlow_RegisterForgotPasswordResetPasswordLogin_WorksCorrectly()
    {
        // Register
        var registerDto = new RegisterDto
        {
            Email = "passwordreset@example.com",
            UserName = "passwordresetuser",
            Password = "Password123!",
            PhoneNumber = "1234567894"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        // Forgot Password
        var forgotPasswordDto = new ForgotPasswordDto
        {
            Email = "passwordreset@example.com"
        };

        await _fixture.PostAsync<ForgotPasswordDto, object>("/api/auth/forgot-password", forgotPasswordDto);

        // Reset Password (token would come from email in real scenario)
        var resetPasswordDto = new ResetPasswordDto
        {
            Email = "passwordreset@example.com",
            Token = "valid-reset-token",
            NewPassword = "NewPassword123!"
        };

        await _fixture.PostAsync<ResetPasswordDto, object>("/api/auth/reset-password", resetPasswordDto);

        // Login with new password
        var loginDto = new LoginDto
        {
            Email = "passwordreset@example.com",
            Password = "NewPassword123!"
        };

        var loginResponse = await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto);
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.AccessToken);
    }

    [Fact]
    public async Task AuthFlow_MultipleLoginsFromSameUser_EachGetsValidToken()
    {
        // Arrange - Register a user
        var registerDto = new RegisterDto
        {
            Email = "multilogin@example.com",
            UserName = "multiloginuser",
            Password = "Password123!",
            PhoneNumber = "1234567890"
        };

        await _fixture.PostAsync<RegisterDto, AuthResponse>("/api/auth/register", registerDto);

        var loginDto = new LoginDto
        {
            Email = "multilogin@example.com",
            Password = "Password123!"
        };

        // Act - Login multiple times
        var loginResponse1 = await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto);
        var loginResponse2 = await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto);
        var loginResponse3 = await _fixture.PostAsync<LoginDto, AuthResponse>("/api/auth/login", loginDto);

        // Assert - All logins should succeed with valid tokens
        Assert.NotNull(loginResponse1.AccessToken);
        Assert.NotNull(loginResponse2.AccessToken);
        Assert.NotNull(loginResponse3.AccessToken);

        // Tokens should be different
        Assert.NotEqual(loginResponse1.AccessToken, loginResponse2.AccessToken);
        Assert.NotEqual(loginResponse2.AccessToken, loginResponse3.AccessToken);
    }

    #endregion
}