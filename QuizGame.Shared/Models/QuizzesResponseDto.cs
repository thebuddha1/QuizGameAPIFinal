namespace QuizGame.Shared.Models;
public class QuizzesResponseDto
{
    public List<QuizListItemDto> ActiveQuizzes { get; set; } = new();
    public List<QuizListItemDto> OtherQuizzes { get; set; } = new();
}