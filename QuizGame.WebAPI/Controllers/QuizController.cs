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
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;
    private readonly IQuizSessionService _sessionService;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public QuizController(
        IQuizService quizService,
        IQuizSessionService sessionService,
        IMapper mapper,
        UserManager<User> userManager)
    {
        _quizService = quizService;
        _sessionService = sessionService;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<QuizzesResponseDto>> GetAll()
    {
        var quizzes = await _quizService.GetAllAsync();

        var active = new List<Quiz>();
        var other = new List<Quiz>();

        foreach (var quiz in quizzes)
        {
            var sessions = await _sessionService.GetByQuizIdAsync(quiz.Id);

            if (sessions.Any(s => s.State == QuizSessionState.Waiting))
                active.Add(quiz);
            else
                other.Add(quiz);
        }

        var result = new QuizzesResponseDto
        {
            ActiveQuizzes = _mapper.Map<List<QuizListItemDto>>(active),
            OtherQuizzes = _mapper.Map<List<QuizListItemDto>>(other)
        };

        return Ok(result);
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<List<QuizListItemDto>>> GetMyQuizzes()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
            return Unauthorized();

        var quizzes = await _quizService.GetAllAsync();

        var my = quizzes.Where(q => q.UserId == userId);

        return Ok(_mapper.Map<List<QuizListItemDto>>(my));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuizDetailsDto>> GetById(int id)
    {
        var quiz = await _quizService.GetByIdAsync(id);

        if (quiz == null)
            return NotFound();

        return Ok(_mapper.Map<QuizDetailsDto>(quiz));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateQuizDto model)
    {
        if (model.Questions == null || !model.Questions.Any())
            return BadRequest("At least one question required");

        foreach (var q in model.Questions)
        {
            if (q.Answers == null || q.Answers.Count < 2)
                return BadRequest("Each question needs at least 2 answers");

            if (!q.Answers.Any(a => a.IsCorrect))
                return BadRequest("Each question must have at least one correct answer");
        }

        for (int i = 0; i < model.Questions.Count; i++)
        {
            model.Questions[i].Order = i;
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null)
            return Unauthorized();

        var quiz = _mapper.Map<Quiz>(model);
        quiz.UserId = userId;

        var created = await _quizService.CreateAsync(quiz);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.Id);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _quizService.DeleteAsync(id);
        return NoContent();
    }
}