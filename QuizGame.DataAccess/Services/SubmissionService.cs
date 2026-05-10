using Microsoft.EntityFrameworkCore;
using QuizGame.DataAccess.Exceptions;
using QuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Services;

public class SubmissionService : ISubmissionService
{
    private readonly QuizGameDbContext _context;

    public SubmissionService(QuizGameDbContext context)
    {
        _context = context;
    }

    public async Task<Submission> SubmitAnswerAsync(int participantId, int questionId, int answerOptionId)
    {
        var exists = await _context.Submissions
            .AnyAsync(s => s.ParticipantId == participantId && s.QuestionId == questionId);

        if (exists)
        {
            throw new Exception("Already answered this question");
        }

        var answer = await _context.AnswersOptions.FindAsync(answerOptionId);

        if (answer == null)
        {
            throw new Exception("Invalid answer");
        }

        var submission = new Submission
        {
            ParticipantId = participantId,
            QuestionId = questionId,
            AnswerOptionId = answerOptionId,
            SubmittedAt = DateTime.UtcNow,
            IsCorrect = answer.IsCorrect
        };

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        return submission;
    }

    public async Task<List<Submission>> GetByQuestionAsync(int questionId)
    {
        var submission = await _context.Submissions
            .Include(s => s.Participant)
            .Where(s => s.QuestionId == questionId)
            .ToListAsync();
        if(submission is null)
        {
            throw new EntityNotFoundException(nameof(Submission));
        }
        return submission;
    }

    public async Task<List<Submission>> GetByParticipantAsync(int participantId)
    {
        var submission = await _context.Submissions
            .Where(s => s.ParticipantId == participantId)
            .ToListAsync();
        if (submission is null)
        {
            throw new EntityNotFoundException(nameof(Submission));
        }
        return submission;
    }

    public async Task<Submission?> GetByParticipantAndQuestionAsync(int participantId, int questionId)
    {
        return await _context.Submissions
            .FirstOrDefaultAsync(s =>
                s.ParticipantId == participantId &&
                s.QuestionId == questionId);
    }
}