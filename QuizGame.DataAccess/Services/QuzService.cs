using Microsoft.EntityFrameworkCore;
using QuizGame.DataAccess.Exceptions;
using QuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Services;
public class QuizService : IQuizService
{
    private readonly QuizGameDbContext _context;

    public QuizService(QuizGameDbContext context)
    {
        _context = context;
    }

    public async Task<List<Quiz>> GetAllAsync()
    {
        var quizzes = await _context.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.AnswerOptions)
            .ToListAsync();

        return quizzes;
    }

    public async Task<Quiz?> GetByIdAsync(int id)
    {
        var quiz = await _context.Quizzes
            .Include(q => q.Questions)
                .ThenInclude(q => q.AnswerOptions)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (quiz is null)
        {
            throw new EntityNotFoundException(nameof(Quiz));
        }

        return quiz;
    }

    public async Task<Quiz> CreateAsync(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();
        return quiz;
    }

    public async Task UpdateAsync(Quiz quiz)
    {
        _context.Quizzes.Update(quiz);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var quiz = await _context.Quizzes.FindAsync(id);
        if (quiz == null) return;

        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();
    }
}