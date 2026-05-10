namespace QuizGame.Shared.Models;

public class JoinQuizRequestDto
{
    public int QuizId { get; set; }
    public string PinCode { get; set; } = null!;
    public string? Nickname { get; set; }
}