using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizGame.DataAccess.Exceptions;
using QuizGame.DataAccess.Models;

namespace QuizGame.DataAccess.Services;

public class ParticipantService : IParticipantService
{
    private readonly QuizGameDbContext _context;

    private readonly UserManager<User> _userManager;

    public ParticipantService(
        QuizGameDbContext context,
        UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Participant> JoinAsync(
        int sessionId,
        string? nickname,
        string? userId)
    {
        if (!string.IsNullOrWhiteSpace(userId))
        {
            var existing =
                await _context.Participants
                    .FirstOrDefaultAsync(p =>
                        p.QuizSessionId == sessionId &&
                        p.UserId == userId);

            if (existing != null)
            {
                return existing;
            }
        }

        var participantCount =
            await _context.Participants
                .CountAsync(p =>
                    p.QuizSessionId == sessionId);

        if (string.IsNullOrWhiteSpace(nickname))
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user =
                    await _userManager.FindByIdAsync(userId);

                nickname =
                    user?.UserName ??
                    $"Guest {participantCount + 1}";
            }
            else
            {
                nickname = $"Guest {participantCount + 1}";
            }
        }

        var participant = new Participant
        {
            QuizSessionId = sessionId,
            UserId = userId,
            Nickname = nickname,
            JoinedAt = DateTime.UtcNow
        };

        _context.Participants.Add(participant);

        await _context.SaveChangesAsync();

        return participant;
    }

    public async Task<List<Participant>> GetBySessionAsync(int sessionId)
    {
        var participants =
            await _context.Participants
                .Where(p => p.QuizSessionId == sessionId)
                .ToListAsync();

        return participants;
    }

    public async Task<Participant?> GetByIdAsync(int id)
    {
        var participant =
            await _context.Participants.FindAsync(id);

        if (participant is null)
        {
            throw new EntityNotFoundException(
                nameof(Participant));
        }

        return participant;
    }
}