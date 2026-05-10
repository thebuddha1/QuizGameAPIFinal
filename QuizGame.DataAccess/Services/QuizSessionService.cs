using Microsoft.EntityFrameworkCore;
using QuizGame.DataAccess.Exceptions;
using QuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Services;
public class QuizSessionService : IQuizSessionService
{
    private readonly QuizGameDbContext _context;

    public QuizSessionService(QuizGameDbContext context)
    {
        _context = context;
    }

    public async Task<QuizSession?> GetByIdAsync(int id)
    {
        var session = await _context.QuizSessions
            .Include(s => s.Quiz)
                .ThenInclude(q => q.Questions)
                    .ThenInclude(q => q.AnswerOptions)
            .Include(s => s.Participants)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session is null)
        {
            throw new EntityNotFoundException(nameof(QuizSession));
        }

        return session;
    }

    public async Task<List<QuizSession>> GetByQuizIdAsync(int quizId)
    {
        var session = await _context.QuizSessions
            .Where(s => s.QuizId == quizId)
            .ToListAsync();
        if (session is null)
        {
            throw new EntityNotFoundException(nameof(QuizSession));
        }
        return session;
    }

    public async Task LockQuestionAsync(int sessionId)
    {
        var session = await _context.QuizSessions.FindAsync(sessionId);
        if (session == null) return;

        session.IsQuestionLocked = true;
        await _context.SaveChangesAsync();
    }

    public async Task NextQuestionAsync(int sessionId)
    {
        var session = await _context.QuizSessions
            .Include(s => s.Quiz)
            .ThenInclude(q => q.Questions)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null) return;

        var questions = session.Quiz.Questions
            .OrderBy(q => q.Order)
            .ToList();

        var currentIndex = questions.FindIndex(q => q.Id == session.CurrentQuestionId);

        if (currentIndex + 1 >= questions.Count)
        {
            session.State = QuizSessionState.Finished;
            session.CurrentQuestionId = null;
            session.IsQuestionLocked = false;
        }
        else
        {
            session.CurrentQuestionId = questions[currentIndex + 1].Id;
            session.IsQuestionLocked = false;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<QuizSession> CreateAsync(int quizId)
    {
        var firstQuestion = await _context.Questions
            .Where(q => q.QuizId == quizId)
            .OrderBy(q => q.Order)
            .FirstOrDefaultAsync();

        var session = new QuizSession
        {
            QuizId = quizId,
            StartedAt = DateTime.UtcNow,
            State = QuizSessionState.Waiting,
            CurrentQuestionId = firstQuestion?.Id
        };

        _context.QuizSessions.Add(session);
        await _context.SaveChangesAsync();

        return session;
    }

    public async Task StartSessionAsync(int sessionId)
    {
        var session = await _context.QuizSessions.FindAsync(sessionId);
        if (session == null) return;

        session.State = QuizSessionState.InProgress;
        await _context.SaveChangesAsync();
    }

    public async Task SetCurrentQuestionAsync(int sessionId, int questionId)
    {
        var session = await _context.QuizSessions.FindAsync(sessionId);
        if (session == null) return;

        session.CurrentQuestionId = questionId;
        await _context.SaveChangesAsync();
    }

    public async Task CloseSessionAsync(int sessionId)
    {
        var session = await _context.QuizSessions
            .Include(s => s.Quiz)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null) return;

        session.State = QuizSessionState.Finished;
        session.EndedAt = DateTime.UtcNow;
        session.Quiz.Status = QuizState.Finished;

        await _context.SaveChangesAsync();
    }
}