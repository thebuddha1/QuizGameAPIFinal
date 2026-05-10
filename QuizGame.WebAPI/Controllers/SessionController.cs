using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizGame.DataAccess.Models;
using QuizGame.DataAccess.Services;
using QuizGame.Shared.Models;

namespace QuizGame.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{
    private readonly IQuizSessionService _sessionService;
    private readonly IQuizService _quizService;
    private readonly ISubmissionService _submissionService;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public SessionController(
        IQuizSessionService sessionService,
        IQuizService quizService,
        ISubmissionService submissionService,
        UserManager<User> userManager,
        IMapper mapper)
    {
        _sessionService = sessionService;
        _quizService = quizService;
        _submissionService = submissionService;
        _userManager = userManager;
        _mapper = mapper;
    }

    [Authorize]
    [HttpPost("open/{quizId}")]
    public async Task<ActionResult<int>> Open(int quizId)
    {
        var session = await _sessionService.CreateAsync(quizId);

        var quiz = await _quizService.GetByIdAsync(quizId);
        if (quiz != null)
        {
            quiz.Status = QuizState.Active;
            await _quizService.UpdateAsync(quiz);
        }

        return Ok(session.Id);
    }

    [HttpGet("{sessionId}")]
    public async Task<ActionResult<QuizSessionDto>> Get(int sessionId, [FromQuery] int? participantId)
    {
        var session = await _sessionService.GetByIdAsync(sessionId);

        if (session == null)
            return NotFound();

        var dto = new QuizSessionDto
        {
            SessionId = session.Id,
            Title = session.Quiz.Title,
            IsStarted = session.State == QuizSessionState.InProgress,
            IsFinished = session.State == QuizSessionState.Finished,
            ParticipantsCount = session.Participants.Count,
            IsLocked = session.IsQuestionLocked
        };

        var currentQuestion = session.Quiz.Questions
            .FirstOrDefault(q => q.Id == session.CurrentQuestionId);

        if (currentQuestion != null)
        {
            dto.CurrentQuestion = _mapper.Map<ActiveQuestionDto>(currentQuestion);
        }

        var userId = _userManager.GetUserId(User);
        dto.IsOwner = session.Quiz.UserId == userId;

        if (!dto.IsOwner && participantId != null)
        {
            dto.ParticipantId = participantId;

            if (session.CurrentQuestionId != null)
            {
                var submission = await _submissionService
                    .GetByParticipantAndQuestionAsync(participantId.Value, session.CurrentQuestionId.Value);

                if (submission != null)
                {
                    dto.HasAnswered = true;
                    dto.SelectedAnswerId = submission.AnswerOptionId;
                    dto.IsCorrect = submission.IsCorrect;
                }
            }
        }

        return Ok(dto);
    }

    [HttpPost("{sessionId}/start")]
    public async Task<IActionResult> Start(int sessionId)
    {
        await _sessionService.StartSessionAsync(sessionId);
        return NoContent();
    }

    [HttpPost("{sessionId}/lock")]
    public async Task<IActionResult> Lock(int sessionId)
    {
        await _sessionService.LockQuestionAsync(sessionId);
        return NoContent();
    }

    [HttpPost("{sessionId}/next")]
    public async Task<IActionResult> Next(int sessionId)
    {
        await _sessionService.NextQuestionAsync(sessionId);
        return NoContent();
    }

    [HttpPost("{sessionId}/end")]
    public async Task<IActionResult> End(int sessionId)
    {
        await _sessionService.CloseSessionAsync(sessionId);
        return NoContent();
    }
}