using QuizGame.Shared.Models;
using QuizGame.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace QuizGame.Tests.Controllers;

public class UsersControllerTests
    : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersControllerTests(
        TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ReturnsCreated_WhenValid()
    {
        var dto = new UserRequestDto
        {
            Name = "Test User",
            Email =
                $"{Guid.NewGuid()}@test.com",
            Password = "Test123@"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/user/register",
                dto);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<UserResponseDto>();

        Assert.NotNull(result);

        Assert.Equal(
            dto.Email,
            result!.Email);
    }

    [Fact]
    public async Task Register_ReturnsConflict_WhenEmailAlreadyExists()
    {
        var email =
            $"{Guid.NewGuid()}@test.com";

        var dto = new UserRequestDto
        {
            Name = "Test User",
            Email = email,
            Password = "Test123@"
        };

        await _client.PostAsJsonAsync(
            "/api/user/register",
            dto);

        var response =
            await _client.PostAsJsonAsync(
                "/api/user/register",
                dto);

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsValid()
    {
        var registerDto = new UserRequestDto
        {
            Name = "Login User",
            Email =
                $"{Guid.NewGuid()}@test.com",
            Password = "Test123@"
        };

        await _client.PostAsJsonAsync(
            "/api/user/register",
            registerDto);

        var loginDto = new LoginRequestDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/user/login",
                loginDto);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(result);

        Assert.False(
            string.IsNullOrWhiteSpace(
                result!.AuthToken));
    }

    [Fact]
    public async Task Login_ReturnsForbidden_WhenPasswordInvalid()
    {
        var registerDto = new UserRequestDto
        {
            Name = "Bad Login User",
            Email =
                $"{Guid.NewGuid()}@test.com",
            Password = "Test123@"
        };

        await _client.PostAsJsonAsync(
            "/api/user/register",
            registerDto);

        var loginDto = new LoginRequestDto
        {
            Email = registerDto.Email,
            Password = "WrongPassword"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/user/login",
                loginDto);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task GetUser_ReturnsUnauthorized_WithoutToken()
    {
        var response =
            await _client.GetAsync(
                "/api/user/test");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetUser_ReturnsOk_WithValidToken()
    {
        var registerDto = new UserRequestDto
        {
            Name = "Authorized User",
            Email =
                $"{Guid.NewGuid()}@test.com",
            Password = "Test123@"
        };

        var registerResponse =
            await _client.PostAsJsonAsync(
                "/api/user/register",
                registerDto);

        var createdUser =
            await registerResponse.Content
                .ReadFromJsonAsync<UserResponseDto>();

        var loginDto = new LoginRequestDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        var loginResponse =
            await _client.PostAsJsonAsync(
                "/api/user/login",
                loginDto);

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponseDto>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult!.AuthToken);

        var response =
            await _client.GetAsync(
                $"/api/user/{createdUser!.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<UserResponseDto>();

        Assert.NotNull(result);

        Assert.Equal(
            registerDto.Email,
            result!.Email);
    }
}