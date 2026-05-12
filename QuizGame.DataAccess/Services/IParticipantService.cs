using QuizGame.DataAccess.Models;

namespace QuizGame.DataAccess.Services;

public interface IParticipantService
{
    Task<Participant> JoinAsync(
        int sessionId,
        string? nickname,
        string? userId);

    Task<List<Participant>> GetBySessionAsync(int sessionId);

    Task<Participant?> GetByIdAsync(int id);
}