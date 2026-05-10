using Microsoft.EntityFrameworkCore;
using QuizGame.DataAccess.Exceptions;
using QuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Services;
public class ParticipantService : IParticipantService
{
    private readonly QuizGameDbContext _context;

    public ParticipantService(QuizGameDbContext context)
    {
        _context = context;
    }

    public async Task<Participant> JoinAsync(int sessionId, string? nickname)
    {
        var participant = new Participant
        {
            QuizSessionId = sessionId,
            Nickname = nickname,
            JoinedAt = DateTime.UtcNow
        };

        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        return participant;
    }

    public async Task<List<Participant>> GetBySessionAsync(int sessionId)
    {
        var participant = await _context.Participants
            .Where(p => p.QuizSessionId == sessionId)
            .ToListAsync();
        if (participant is null)
        {
            throw new EntityNotFoundException(nameof(Participant));
        }
        return participant;
    }

    public async Task<Participant?> GetByIdAsync(int id)
    {
        var participant = await _context.Participants.FindAsync(id);
        if (participant is null)
        {
            throw new EntityNotFoundException(nameof(Participant));
        }
        return participant;
    }
}