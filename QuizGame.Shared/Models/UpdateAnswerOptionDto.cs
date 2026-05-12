namespace QuizGame.Shared.Models;

public class UpdateAnswerOptionDto
{
    public int? Id { get; set; }

    public string Text { get; set; } = null!;

    public bool IsCorrect { get; set; }
}