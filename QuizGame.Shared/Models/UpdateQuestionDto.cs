namespace QuizGame.Shared.Models;

public class UpdateQuestionDto
{
    public int? Id { get; set; }

    public string Text { get; set; } = null!;

    public int Order { get; set; }

    public List<UpdateAnswerOptionDto> Answers { get; set; } = new();
}