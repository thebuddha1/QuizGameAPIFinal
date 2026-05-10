using QuizGame.DataAccess.Models;

namespace QuizGame.DataAccess.Services;
public interface IQuizService
{
    Task<List<Quiz>> GetAllAsync();
    Task<Quiz?> GetByIdAsync(int id);

    Task<Quiz> CreateAsync(Quiz quiz);
    Task UpdateAsync(Quiz quiz);
    Task DeleteAsync(int id);
}