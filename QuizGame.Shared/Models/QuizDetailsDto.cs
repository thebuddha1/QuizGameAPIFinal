namespace QuizGame.Shared.Models;
public class QuizDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string PinCode { get; set; } = null!;
    public bool HasActiveSession { get; set; }
    public int? ActiveSessionId { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}