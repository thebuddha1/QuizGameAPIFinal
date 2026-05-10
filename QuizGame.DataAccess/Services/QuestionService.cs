using Microsoft.EntityFrameworkCore;
using QuizGame.DataAccess.Exceptions;
using QuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Services;
public class QuestionService : IQuestionService
{
    private readonly QuizGameDbContext _context;

    public QuestionService(QuizGameDbContext context)
    {
        _context = context;
    }

    public async Task<List<Question>> GetByQuizIdAsync(int quizId)
    {
        var questions = await _context.Questions
            .Include(q => q.AnswerOptions)
            .Where(q => q.QuizId == quizId)
            .OrderBy(q => q.Order)
            .ToListAsync();
        return questions;
    }

    public async Task<Question?> GetByIdAsync(int id)
    {
        var question = await _context.Questions
            .Include(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (question is null)
        {
            throw new EntityNotFoundException(nameof(Question));
        }
        return question;
    }

    public async Task<Question> CreateAsync(Question question)
    {
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task UpdateAsync(Question question)
    {
        _context.Questions.Update(question);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question == null) return;

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
    }
}