namespace QuizGame.Shared.Models;
public class CreateAnswerOptionDto
{
    public string Text { get; set; } = null!;
    public bool IsCorrect { get; set; }
}