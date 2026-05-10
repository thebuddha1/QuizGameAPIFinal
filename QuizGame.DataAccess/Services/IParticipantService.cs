using QuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Services;
public interface IParticipantService
{
    Task<Participant> JoinAsync(int sessionId, string? nickname);

    Task<List<Participant>> GetBySessionAsync(int sessionId);
    Task<Participant?> GetByIdAsync(int id);
}