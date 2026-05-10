using Microsoft.AspNetCore.Mvc;
using QuizGame.DataAccess.Models;
using QuizGame.DataAccess.Services;
using QuizGame.Shared.Models;

namespace QuizGame.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IQuizService _quizService;
    private readonly IQuizSessionService _sessionService;
    private readonly IParticipantService _participantService;
    private readonly ISubmissionService _submissionService;

    public PlayerController(
        IQuizService quizService,
        IQuizSessionService sessionService,
        IParticipantService participantService,
        ISubmissionService submissionService)
    {
        _quizService = quizService;
        _sessionService = sessionService;
        _participantService = participantService;
        _submissionService = submissionService;
    }

    [HttpPost("join")]
    public async Task<ActionResult<JoinQuizResponseDto>> Join([FromBody] JoinQuizRequestDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var quiz = await _quizService.GetByIdAsync(model.QuizId);
        if (quiz == null)
            return NotFound("Quiz not found");

        if (quiz.PinCode != model.PinCode)
            return BadRequest("Invalid PIN code");

        var session = (await _sessionService.GetByQuizIdAsync(model.QuizId))
            .FirstOrDefault(s => s.State == QuizSessionState.Waiting);

        if (session == null)
            return BadRequest("Quiz is not joinable");

        var participant = await _participantService.JoinAsync(session.Id, model.Nickname);

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

        return NoContent();
    }
}