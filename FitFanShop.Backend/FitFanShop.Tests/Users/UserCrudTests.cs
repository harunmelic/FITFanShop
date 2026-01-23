using System.Net;
using System.Net.Http.Json;
using FitFanShop.Application.Common;
using FitFanShop.Application.Modules.Auth.Commands.Login;
using FitFanShop.Application.Modules.Auth.Commands.Register;
using FitFanShop.Application.Modules.Users;
using FitFanShop.Application.Modules.Users.Commands.ChangePassword;
using FitFanShop.Application.Modules.Users.Commands.UpdateMyProfile;
using Xunit;

namespace FitFanShop.Tests.Users;

public class UserCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UserCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task User_Register_CreatesNewUser()
    {
        var client = _factory.CreateClient();

        var registerCommand = new RegisterCommand
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "TestPass123!",
            ConfirmPassword = "TestPass123!"
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", registerCommand);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(loginResponse);
        Assert.NotEmpty(loginResponse.AccessToken);
        Assert.NotEmpty(loginResponse.RefreshToken);
    }

    [Fact]
    public async Task User_GetMyProfile_ReturnsProfile()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/users/me");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        Assert.NotNull(profile);
        Assert.NotEmpty(profile.Email);
        Assert.NotEmpty(profile.FirstName);
    }

    [Fact]
    public async Task User_UpdateMyProfile_UpdatesProfile()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var updateCommand = new UpdateMyProfileCommand
        {
            FirstName = "Updated",
            LastName = "Name"
        };

        var response = await client.PutAsJsonAsync("/api/users/me", updateCommand);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        Assert.NotNull(profile);
        Assert.Equal("Updated", profile.FirstName);
        Assert.Equal("Name", profile.LastName);
    }

    [Fact]
    public async Task User_ChangePassword_ChangesPassword()
    {
        var email = $"changepass{Guid.NewGuid()}@example.com";
        var oldPassword = "OldPass123!";
        var newPassword = "NewPass456!";

        var client = _factory.CreateClient();

        await client.PostAsJsonAsync("/api/auth/register", new RegisterCommand
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = oldPassword,
            ConfirmPassword = oldPassword
        });

        await Task.Delay(100);

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginCommand
        {
            Email = email,
            Password = oldPassword
        });
        
        var tokens = await loginResponse.Content.ReadFromJsonAsync<LoginCommandDto>();
        Assert.NotNull(tokens);
        
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var changePasswordCommand = new ChangePasswordCommand
        {
            CurrentPassword = oldPassword,
            NewPassword = newPassword,
            ConfirmNewPassword = newPassword
        };

        var changeResponse = await client.PostAsJsonAsync("/api/users/me/change-password", changePasswordCommand);
        Assert.Equal(HttpStatusCode.NoContent, changeResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = null;
        
        await Task.Delay(100);
        
        var newLoginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginCommand
        {
            Email = email,
            Password = newPassword
        });
        Assert.Equal(HttpStatusCode.OK, newLoginResponse.StatusCode);
    }

    [Fact]
    public async Task User_DeleteMyAccount_DeletesAccount()
    {
        var client = _factory.CreateClient();

        var email = $"delete{Guid.NewGuid()}@example.com";
        var password = "DeletePass123!";

        await client.PostAsJsonAsync("/api/auth/register", new RegisterCommand
        {
            FirstName = "Delete",
            LastName = "Test",
            Email = email,
            Password = password,
            ConfirmPassword = password
        });

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginCommand
        {
            Email = email,
            Password = password
        });
        var tokens = await loginResponse.Content.ReadFromJsonAsync<LoginCommandDto>();
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

        var deleteResponse = await client.DeleteAsync("/api/users/me");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = null;
        var loginAfterDeleteResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginCommand
        {
            Email = email,
            Password = password
        });
        Assert.Equal(HttpStatusCode.NotFound, loginAfterDeleteResponse.StatusCode);
    }

    [Fact]
    public async Task Admin_GetAllUsers_ReturnsPaginatedUsers()
    {
        var client = await _factory.GetAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/users?page=1&pageSize=5");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var users = await response.Content.ReadFromJsonAsync<PageResult<UserDto>>();
        Assert.NotNull(users);
        Assert.NotEmpty(users.Items);
    }

    [Fact]
    public async Task Admin_UpdateUserEnabled_DisablesUser()
    {
        var client = _factory.CreateClient();

        var email = $"disable{Guid.NewGuid()}@example.com";
        var password = "DisablePass123!";

        await client.PostAsJsonAsync("/api/auth/register", new RegisterCommand
        {
            FirstName = "Disable",
            LastName = "Test",
            Email = email,
            Password = password,
            ConfirmPassword = password
        });

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginCommand
        {
            Email = email,
            Password = password
        });
        var userTokens = await loginResponse.Content.ReadFromJsonAsync<LoginCommandDto>();

        var adminClient = await _factory.GetAuthenticatedClientAsync();
        
        var allUsersResponse = await adminClient.GetAsync("/api/users");
        var allUsers = await allUsersResponse.Content.ReadFromJsonAsync<PageResult<UserDto>>();
        var targetUser = allUsers!.Items.FirstOrDefault(u => u.Email == email);
        
        if (targetUser != null)
        {
            var disableResponse = await adminClient.PatchAsJsonAsync($"/api/users/{targetUser.Id}/enabled", 
                new { isEnabled = false });
            Assert.Equal(HttpStatusCode.NoContent, disableResponse.StatusCode);

            client.DefaultRequestHeaders.Authorization = null;
            var loginAfterDisableResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginCommand
            {
                Email = email,
                Password = password
            });
            Assert.Equal(HttpStatusCode.NotFound, loginAfterDisableResponse.StatusCode);
        }
    }
}
