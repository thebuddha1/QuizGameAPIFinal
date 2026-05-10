namespace QuizGame.Shared.Models;

public class ActiveQuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public List<AnswerOptionDto> Answers { get; set; } = new();
}