namespace QuizGame.Shared.Models;

public class UpdateQuizDto
{
    public string Title { get; set; } = null!;

    public List<UpdateQuestionDto> Questions { get; set; } = new();
}