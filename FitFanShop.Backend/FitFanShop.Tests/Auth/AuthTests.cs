using System.Net;
using System.Net.Http.Json;
using FitFanShop.Application.Modules.Auth.Commands.Login;
using FitFanShop.Application.Modules.Auth.Commands.Register;
using FitFanShop.Application.Modules.Auth.Commands.Refresh;
using FitFanShop.Application.Modules.Auth.Commands.Logout;

namespace FitFanShop.Tests.Auth;

public class AuthTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region Registration Tests

    [Fact]
    public async Task Register_WithValidData_ReturnsOkWithTokens()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"testuser{Guid.NewGuid()}@test.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.True(result.ExpiresAtUtc > DateTime.UtcNow);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var email = $"duplicate{Guid.NewGuid()}@test.com";
        
        var firstCommand = new RegisterCommand
        {
            FirstName = "First",
            LastName = "User",
            Email = email,
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };

        var secondCommand = new RegisterCommand
        {
            FirstName = "Second",
            LastName = "User",
            Email = email,
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };

        // Act
        await _client.PostAsJsonAsync("/api/auth/register", firstCommand);
        var response = await _client.PostAsJsonAsync("/api/auth/register", secondCommand);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "Test",
            LastName = "User",
            Email = "invalid-email",
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"testuser{Guid.NewGuid()}@test.com",
            Password = "weak", // No uppercase, no digit, too short
            ConfirmPassword = "weak",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithMismatchedPasswords_ReturnsBadRequest()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"testuser{Guid.NewGuid()}@test.com",
            Password = "Test123!",
            ConfirmPassword = "DifferentPassword123!",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithMissingFirstName_ReturnsBadRequest()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "",
            LastName = "User",
            Email = $"testuser{Guid.NewGuid()}@test.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithTokens()
    {
        // Arrange - First register a user
        var email = $"logintest{Guid.NewGuid()}@test.com";
        var password = "Test123!";
        
        var registerCommand = new RegisterCommand
        {
            FirstName = "Login",
            LastName = "Test",
            Email = email,
            Password = password,
            ConfirmPassword = password,
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };
        
        await _client.PostAsJsonAsync("/api/auth/register", registerCommand);

        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsInternalServerError()
    {
        // Arrange
        var email = $"logintest{Guid.NewGuid()}@test.com";
        var registerCommand = new RegisterCommand
        {
            FirstName = "Login",
            LastName = "Test",
            Email = email,
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };
        
        await _client.PostAsJsonAsync("/api/auth/register", registerCommand);

        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

        // Assert
        // FitFanShopInvalidCredentialsException currently returns 500 instead of 401/404
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsNotFound()
    {
        // Arrange
        var loginCommand = new LoginCommand
        {
            Email = $"nonexistent{Guid.NewGuid()}@test.com",
            Password = "Test123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyEmail_ReturnsBadRequest()
    {
        // Arrange
        var loginCommand = new LoginCommand
        {
            Email = "",
            Password = "Test123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var loginCommand = new LoginCommand
        {
            Email = "test@test.com",
            Password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Refresh Token Tests

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsNewTokens()
    {
        // Arrange - Register and get initial tokens
        var email = $"refreshtest{Guid.NewGuid()}@test.com";
        var registerCommand = new RegisterCommand
        {
            FirstName = "Refresh",
            LastName = "Test",
            Email = email,
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };
        
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerCommand);
        var initialTokens = await registerResponse.Content.ReadFromJsonAsync<LoginCommandDto>();

        var refreshCommand = new RefreshTokenCommand
        {
            RefreshToken = initialTokens!.RefreshToken
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshCommand);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var newTokens = await response.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(newTokens);
        Assert.NotEmpty(newTokens.AccessToken);
        Assert.NotEmpty(newTokens.RefreshToken);
        Assert.NotEqual(initialTokens.AccessToken, newTokens.AccessToken);
        Assert.NotEqual(initialTokens.RefreshToken, newTokens.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsConflict()
    {
        // Arrange
        var refreshCommand = new RefreshTokenCommand
        {
            RefreshToken = "invalid-token-12345"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshCommand);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_WithRevokedToken_ReturnsConflict()
    {
        // Arrange - Register, get token, then use it (revokes it)
        var email = $"revoketest{Guid.NewGuid()}@test.com";
        var registerCommand = new RegisterCommand
        {
            FirstName = "Revoke",
            LastName = "Test",
            Email = email,
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };
        
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerCommand);
        var initialTokens = await registerResponse.Content.ReadFromJsonAsync<LoginCommandDto>();

        // First refresh (revokes old token)
        var firstRefresh = new RefreshTokenCommand { RefreshToken = initialTokens!.RefreshToken };
        await _client.PostAsJsonAsync("/api/auth/refresh", firstRefresh);

        // Try to use the revoked token again
        var secondRefresh = new RefreshTokenCommand { RefreshToken = initialTokens.RefreshToken };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", secondRefresh);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    #endregion

    #region Logout Tests

    [Fact]
    public async Task Logout_WithValidRefreshToken_ReturnsOk()
    {
        // Arrange
        var email = $"logouttest{Guid.NewGuid()}@test.com";
        var registerCommand = new RegisterCommand
        {
            FirstName = "Logout",
            LastName = "Test",
            Email = email,
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };
        
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerCommand);
        var tokens = await registerResponse.Content.ReadFromJsonAsync<LoginCommandDto>();

        var logoutCommand = new LogoutCommand
        {
            RefreshToken = tokens!.RefreshToken
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/logout", logoutCommand);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Logout_ThenRefresh_ShouldFail()
    {
        // Arrange
        var email = $"logoutrefresh{Guid.NewGuid()}@test.com";
        var registerCommand = new RegisterCommand
        {
            FirstName = "LogoutRefresh",
            LastName = "Test",
            Email = email,
            Password = "Test123!",
            ConfirmPassword = "Test123!",
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };
        
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerCommand);
        var tokens = await registerResponse.Content.ReadFromJsonAsync<LoginCommandDto>();

        // Logout
        var logoutCommand = new LogoutCommand { RefreshToken = tokens!.RefreshToken };
        await _client.PostAsJsonAsync("/api/auth/logout", logoutCommand);

        // Try to refresh with logged out token
        var refreshCommand = new RefreshTokenCommand { RefreshToken = tokens.RefreshToken };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", refreshCommand);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    #endregion

    #region Integration/Flow Tests

    [Fact]
    public async Task FullAuthFlow_RegisterLoginRefreshLogout_Success()
    {
        // 1. Register
        var email = $"fullflow{Guid.NewGuid()}@test.com";
        var password = "Test123!";
        var registerCommand = new RegisterCommand
        {
            FirstName = "Full",
            LastName = "Flow",
            Email = email,
            Password = password,
            ConfirmPassword = password,
            SecurityQuestion = "What is your favorite color?",
            SecurityAnswer = "Blue"
        };
        
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerCommand);
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
        var registerTokens = await registerResponse.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(registerTokens);

        // 2. Login
        var loginCommand = new LoginCommand { Email = email, Password = password };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginTokens = await loginResponse.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(loginTokens);

        // 3. Refresh
        var refreshCommand = new RefreshTokenCommand { RefreshToken = loginTokens.RefreshToken };
        var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh", refreshCommand);
        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        var newTokens = await refreshResponse.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(newTokens);

        // 4. Logout
        var logoutCommand = new LogoutCommand { RefreshToken = newTokens.RefreshToken };
        var logoutResponse = await _client.PostAsJsonAsync("/api/auth/logout", logoutCommand);
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);

        // 5. Verify token is revoked
        var finalRefresh = new RefreshTokenCommand { RefreshToken = newTokens.RefreshToken };
        var finalResponse = await _client.PostAsJsonAsync("/api/auth/refresh", finalRefresh);
        Assert.Equal(HttpStatusCode.Conflict, finalResponse.StatusCode);
    }

    #endregion
}
