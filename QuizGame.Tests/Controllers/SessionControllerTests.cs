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

public class SessionControllerTests
    : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SessionControllerTests(
        TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> LoginAndGetToken()
    {
        var registerDto = new UserRequestDto
        {
            Name = "Session User",
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

        var loginResponse =
            await _client.PostAsJsonAsync(
                "/api/user/login",
                loginDto);

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponseDto>();

        return loginResult!.AuthToken;
    }

    [Fact]
    public async Task Get_ReturnsNotFound_ForInvalidSession()
    {
        var response =
            await _client.GetAsync(
                "/api/session/99999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsSession_WhenSessionExists()
    {
        using var scope =
            _factory.Services.CreateScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<QuizGameDbContext>();

        var quiz = new Quiz
        {
            Title = "Session Quiz",
            UserId = "owner",
            PinCode = "111111",
            CreatedAt = DateTime.UtcNow,
            Status = QuizState.Draft,
            Questions =
            [
                new Question
                {
                    Text = "Question",
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

        var session = new QuizSession
        {
            QuizId = quiz.Id,
            StartedAt = DateTime.UtcNow,
            State = QuizSessionState.Waiting,
            CurrentQuestionId =
                quiz.Questions.First().Id
        };

        db.QuizSessions.Add(session);

        await db.SaveChangesAsync();

        var response =
            await _client.GetAsync(
                $"/api/session/{session.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<QuizSessionDto>();

        Assert.NotNull(result);

        Assert.Equal(
            "Session Quiz",
            result!.Title);
    }

    [Fact]
    public async Task Open_ReturnsUnauthorized_WithoutToken()
    {
        var response =
            await _client.PostAsync(
                "/api/session/open/1",
                null);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Open_ReturnsOk_WithValidToken()
    {
        var token =
            await LoginAndGetToken();

        using var scope =
            _factory.Services.CreateScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<QuizGameDbContext>();

        var quiz = new Quiz
        {
            Title = "Open Quiz",
            UserId = "owner",
            PinCode = "222222",
            CreatedAt = DateTime.UtcNow,
            Status = QuizState.Draft,
            Questions =
            [
                new Question
                {
                    Text = "Question",
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

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var response =
            await _client.PostAsync(
                $"/api/session/open/{quiz.Id}",
                null);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task Start_ReturnsNoContent_WhenSessionExists()
    {
        using var scope =
            _factory.Services.CreateScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<QuizGameDbContext>();

        var quiz = new Quiz
        {
            Title = "Start Quiz",
            UserId = "owner",
            PinCode = "333333",
            CreatedAt = DateTime.UtcNow,
            Status = QuizState.Draft,
            Questions =
            [
                new Question
                {
                    Text = "Question",
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

        var session = new QuizSession
        {
            QuizId = quiz.Id,
            StartedAt = DateTime.UtcNow,
            State = QuizSessionState.Waiting
        };

        db.QuizSessions.Add(session);

        await db.SaveChangesAsync();

        var response =
            await _client.PostAsync(
                $"/api/session/{session.Id}/start",
                null);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }

    [Fact]
    public async Task Lock_ReturnsNoContent_WhenSessionExists()
    {
        using var scope =
            _factory.Services.CreateScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<QuizGameDbContext>();

        var quiz = new Quiz
        {
            Title = "Lock Quiz",
            UserId = "owner",
            PinCode = "444444",
            CreatedAt = DateTime.UtcNow,
            Status = QuizState.Draft,
            Questions =
            [
                new Question
                {
                    Text = "Question",
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

        var session = new QuizSession
        {
            QuizId = quiz.Id,
            StartedAt = DateTime.UtcNow,
            State = QuizSessionState.InProgress
        };

        db.QuizSessions.Add(session);

        await db.SaveChangesAsync();

        var response =
            await _client.PostAsync(
                $"/api/session/{session.Id}/lock",
                null);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }

    [Fact]
    public async Task End_ReturnsNoContent_WhenSessionExists()
    {
        using var scope =
            _factory.Services.CreateScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<QuizGameDbContext>();

        var quiz = new Quiz
        {
            Title = "End Quiz",
            UserId = "owner",
            PinCode = "555555",
            CreatedAt = DateTime.UtcNow,
            Status = QuizState.Draft,
            Questions =
            [
                new Question
                {
                    Text = "Question",
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

        var session = new QuizSession
        {
            QuizId = quiz.Id,
            StartedAt = DateTime.UtcNow,
            State = QuizSessionState.InProgress
        };

        db.QuizSessions.Add(session);

        await db.SaveChangesAsync();

        var response =
            await _client.PostAsync(
                $"/api/session/{session.Id}/end",
                null);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }
}