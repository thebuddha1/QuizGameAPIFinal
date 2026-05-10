namespace QuizGame.Shared.Models;
public class CreateQuizDto
{
    public string Title { get; set; } = null!;
    public List<CreateQuestionDto> Questions { get; set; } = new();
}