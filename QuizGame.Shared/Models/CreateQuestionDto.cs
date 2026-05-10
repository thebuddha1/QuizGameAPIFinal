namespace QuizGame.Shared.Models;
public class CreateQuestionDto
{
    public string Text { get; set; } = null!;
    public int TimeLimitSeconds { get; set; }
    public int Order { get; set; }
    public List<CreateAnswerOptionDto> Answers { get; set; } = new();
}