using QuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Services;
public interface IQuestionService
{
    Task<List<Question>> GetByQuizIdAsync(int quizId);
    Task<Question?> GetByIdAsync(int id);

    Task<Question> CreateAsync(Question question);
    Task UpdateAsync(Question question);
    Task DeleteAsync(int id);
}