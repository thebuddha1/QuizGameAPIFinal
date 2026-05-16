using Microsoft.Extensions.DependencyInjection;
using QuizGame.DataAccess;
using QuizGame.DataAccess.Models;
using QuizGame.Shared.Models;
using QuizGame.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace QuizGame.Tests.Controllers;

public class QuizControllerTests
    : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public QuizControllerTests(
        TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/quiz");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForInvalidId()
    {
        var response = await _client.GetAsync("/api/quiz/99999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsQuiz_WhenQuizExists()
    {
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<QuizGameDbContext>();

        var quiz = new Quiz
        {
            Title = "Test Quiz",
            UserId = "test-user",
            PinCode = "123456",
            CreatedAt = DateTime.UtcNow,
            Status = QuizState.Draft,
            Questions =
            [
                new Question
                {
                    Text = "Question 1",
                    Order = 0,
                    AnswerOptions =
                    [
                        new AnswerOption
                        {
                            Text = "A",
                            IsCorrect = true
                        },
                        new AnswerOption
                        {
                            Text = "B",
                            IsCorrect = false
                        }
                    ]
                }
            ]
        };

        db.Quizzes.Add(quiz);

        await db.SaveChangesAsync();

        var response = await _client.GetAsync($"/api/quiz/{quiz.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<QuizDetailsDto>();

        Assert.NotNull(result);

        Assert.Equal(
            "Test Quiz",
            result!.Title);
    }

    [Fact]
    public async Task Create_ReturnsUnauthorized_WithoutToken()
    {
        var dto = new CreateQuizDto
        {
            Title = "Quiz",
            Questions =
            [
                new CreateQuestionDto
                {
                    Text = "Question",
                    Order = 0,
                    Answers =
                    [
                        new CreateAnswerOptionDto
                        {
                            Text = "A",
                            IsCorrect = true
                        },
                        new CreateAnswerOptionDto
                        {
                            Text = "B",
                            IsCorrect = false
                        }
                    ]
                }
            ]
        };

        var response = await _client.PostAsJsonAsync("/api/quiz",dto);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsUnauthorized_WithoutToken()
    {
        var response = await _client.DeleteAsync("/api/quiz/1");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithValidToken()
    {
        var token = await RegisterAndLogin();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateQuizDto
        {
            Title = "Created Quiz",
            Questions =
            [
                new CreateQuestionDto
                {
                    Text = "Question",
                    Order = 0,
                    Answers =
                    [
                        new CreateAnswerOptionDto
                        {
                            Text = "Correct",
                            IsCorrect = true
                        },
                        new CreateAnswerOptionDto
                        {
                            Text = "Wrong",
                            IsCorrect = false
                        }
                    ]
                }
            ]
        };

        var response = await _client.PostAsJsonAsync("/api/quiz", dto);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WithValidToken()
    {
        var token = await RegisterAndLogin();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        int quizId;

        using (var scope =_factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<QuizGameDbContext>();

            var user = db.Users.OrderByDescending(u => u.Id).First();

            var quiz = new Quiz
            {
                Title = "Delete Quiz",
                UserId = user.Id,
                PinCode = "123456",
                CreatedAt = DateTime.UtcNow,
                Status = QuizState.Draft
            };

            db.Quizzes.Add(quiz);

            await db.SaveChangesAsync();

            quizId = quiz.Id;
        }

        var response = await _client.DeleteAsync($"/api/quiz/{quizId}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }

    private async Task<string> RegisterAndLogin()
    {
        var registerDto = new UserRequestDto
        {
            Name = Guid.NewGuid().ToString(),
            Email =
                $"{Guid.NewGuid()}@test.com",
            Password = "Test123@"
        };

        var registerResponse =await _client.PostAsJsonAsync("/api/user/register", registerDto);

        registerResponse.EnsureSuccessStatusCode();

        var loginDto = new LoginRequestDto
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/user/login",loginDto);

        loginResponse.EnsureSuccessStatusCode();

        var result = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        return result!.AuthToken;
    }
}