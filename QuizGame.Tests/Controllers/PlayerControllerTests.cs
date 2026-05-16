using Microsoft.Extensions.DependencyInjection;
using QuizGame.DataAccess;
using QuizGame.DataAccess.Models;
using QuizGame.Shared.Models;
using QuizGame.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace QuizGame.Tests.Controllers;

public class PlayerControllerTests
    : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PlayerControllerTests(
        TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Join_ReturnsOk_WhenValid()
    {
        using var scope =
            _factory.Services.CreateScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<QuizGameDbContext>();

        var quiz = new Quiz
        {
            Title = "Join Quiz",
            UserId = "owner",
            PinCode = "123456",
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

        var dto = new JoinQuizRequestDto
        {
            QuizId = quiz.Id,
            PinCode = "123456",
            Nickname = "Player1"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/player/join",
                dto);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync
                    <JoinQuizResponseDto>();

        Assert.NotNull(result);

        Assert.True(
            result!.ParticipantId > 0);
    }

    [Fact]
    public async Task Join_ReturnsBadRequest_WhenPinInvalid()
    {
        using var scope =
            _factory.Services.CreateScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<QuizGameDbContext>();

        var quiz = new Quiz
        {
            Title = "Wrong Pin Quiz",
            UserId = "owner",
            PinCode = "654321",
            CreatedAt = DateTime.UtcNow,
            Status = QuizState.Draft
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

        var dto = new JoinQuizRequestDto
        {
            QuizId = quiz.Id,
            PinCode = "000000",
            Nickname = "Player"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/player/join",
                dto);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Join_ReturnsBadRequest_WhenQuizAlreadyStarted()
    {
        using var scope =
            _factory.Services.CreateScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<QuizGameDbContext>();

        var quiz = new Quiz
        {
            Title = "Started Quiz",
            UserId = "owner",
            PinCode = "111111",
            CreatedAt = DateTime.UtcNow,
            Status = QuizState.Draft
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

        var dto = new JoinQuizRequestDto
        {
            QuizId = quiz.Id,
            PinCode = "111111",
            Nickname = "Player"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/player/join",
                dto);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Submit_ReturnsNoContent_WhenValid()
    {
        using var scope =
            _factory.Services.CreateScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<QuizGameDbContext>();

        var question = new Question
        {
            Text = "Question",
            Order = 0
        };

        var correctAnswer = new AnswerOption
        {
            Text = "Correct",
            IsCorrect = true,
            Question = question
        };

        var wrongAnswer = new AnswerOption
        {
            Text = "Wrong",
            IsCorrect = false,
            Question = question
        };

        question.AnswerOptions =
        [
            correctAnswer,
            wrongAnswer
        ];

        var quiz = new Quiz
        {
            Title = "Submit Quiz",
            UserId = "owner",
            PinCode = "222222",
            CreatedAt = DateTime.UtcNow,
            Status = QuizState.Draft,
            Questions =
            [
                question
            ]
        };

        db.Quizzes.Add(quiz);

        await db.SaveChangesAsync();

        var session = new QuizSession
        {
            QuizId = quiz.Id,
            StartedAt = DateTime.UtcNow,
            State = QuizSessionState.InProgress,
            CurrentQuestionId = question.Id
        };

        db.QuizSessions.Add(session);

        await db.SaveChangesAsync();

        var participant = new Participant
        {
            QuizSessionId = session.Id,
            Nickname = "Player",
            JoinedAt = DateTime.UtcNow
        };

        db.Participants.Add(participant);

        await db.SaveChangesAsync();

        var dto = new SubmitAnswerDto
        {
            SessionId = session.Id,
            ParticipantId = participant.Id,
            QuestionId = question.Id,
            AnswerOptionId = correctAnswer.Id
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/player/submit",
                dto);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }

    [Fact]
    public async Task Submit_ReturnsBadRequest_WhenParticipantInvalid()
    {
        var dto = new SubmitAnswerDto
        {
            SessionId = 1,
            ParticipantId = 0,
            QuestionId = 1,
            AnswerOptionId = 1
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/player/submit",
                dto);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}