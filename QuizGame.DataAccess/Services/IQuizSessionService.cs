using QuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Services;
public interface IQuizSessionService
{
    Task<QuizSession?> GetByIdAsync(int id);
    Task<List<QuizSession>> GetByQuizIdAsync(int quizId);
    Task LockQuestionAsync(int sessionId);
    Task NextQuestionAsync(int sessionId);
    Task<QuizSession> CreateAsync(int quizId);
    Task StartSessionAsync(int sessionId);

    Task SetCurrentQuestionAsync(int sessionId, int questionId);
    Task CloseSessionAsync(int sessionId);
}