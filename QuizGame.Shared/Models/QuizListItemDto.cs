namespace QuizGame.Shared.Models;

public class QuizListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool HasActiveSession { get; set; }
    public int? ActiveSessionId { get; set; }
}