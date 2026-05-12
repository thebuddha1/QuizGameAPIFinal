using Microsoft.AspNetCore.SignalR;

namespace QuizGame.SignalR.Hubs;

public class QuizHub : Hub
{
    public async Task JoinSessionGroup(int sessionId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            $"session-{sessionId}"
        );
    }

    public async Task LeaveSessionGroup(int sessionId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            $"session-{sessionId}"
        );
    }
}