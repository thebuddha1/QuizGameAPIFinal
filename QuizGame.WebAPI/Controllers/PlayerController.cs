using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuizGame.DataAccess.Models;
using QuizGame.DataAccess.Services;
using QuizGame.Shared.Models;
using QuizGame.SignalR;
using QuizGame.SignalR.Hubs;

namespace QuizGame.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IQuizService _quizService;
    private readonly IQuizSessionService _sessionService;
    private readonly IParticipantService _participantService;
    private readonly ISubmissionService _submissionService;

    private readonly IHubContext<QuizHub> _hub;

    public PlayerController(
        IQuizService quizService,
        IQuizSessionService sessionService,
        IParticipantService participantService,
        ISubmissionService submissionService,
        IHubContext<QuizHub> hub)
    {
        _quizService = quizService;
        _sessionService = sessionService;
        _participantService = participantService;
        _submissionService = submissionService;
        _hub = hub;
    }

    [HttpPost("join")]
    public async Task<ActionResult<JoinQuizResponseDto>> Join(
    [FromBody] JoinQuizRequestDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var quiz =
            await _quizService.GetByIdAsync(model.QuizId);

        if (quiz == null)
            return NotFound("Quiz not found");

        var session =
            (await _sessionService
                .GetByQuizIdAsync(model.QuizId))
            .FirstOrDefault(s =>
                s.State == QuizSessionState.Waiting);

        if (session == null)
            return BadRequest("Quiz has already started");

        if (quiz.PinCode != model.PinCode)
            return BadRequest("Invalid PIN code");

        var userId = HttpContext.User.FindFirst("id")?.Value;

        Console.WriteLine("JWT USER ID:");
        Console.WriteLine(userId);

        Console.WriteLine("QUIZ OWNER ID:");
        Console.WriteLine(quiz.UserId);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            if (quiz.UserId == userId)
            {
                return BadRequest(
                    "Owners cannot join their own quiz as participants");
            }
        }

        var participant =
            await _participantService.JoinAsync(
                session.Id,
                model.Nickname,
                userId);

        await BroadcastSessionUpdate(session.Id);

        return Ok(new JoinQuizResponseDto
        {
            SessionId = session.Id,
            ParticipantId = participant.Id
        });
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitAnswerDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (model.ParticipantId <= 0)
            return BadRequest("Invalid participant");

        await _submissionService.SubmitAnswerAsync(
            model.ParticipantId,
            model.QuestionId,
            model.AnswerOptionId
        );

        await BroadcastSessionUpdate(model.SessionId);

        return NoContent();
    }

    private async Task BroadcastSessionUpdate(int sessionId)
    {
        await _hub.Clients
            .Group($"session-{sessionId}")
            .SendAsync(SignalREvents.SessionUpdated);
    }
}