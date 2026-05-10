namespace QuizGame.Shared.Models;

public class QuizSessionDto
{
    public int SessionId { get; set; }

    public string Title { get; set; } = null!;

    public bool IsStarted { get; set; }
    public bool IsFinished { get; set; }

    public int ParticipantsCount { get; set; }

    public bool IsLocked { get; set; }

    public bool IsOwner { get; set; }

    public int? ParticipantId { get; set; }

    public bool HasAnswered { get; set; }
    public int? SelectedAnswerId { get; set; }
    public bool? IsCorrect { get; set; }

    public ActiveQuestionDto? CurrentQuestion { get; set; }
}