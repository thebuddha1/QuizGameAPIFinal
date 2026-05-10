namespace QuizGame.Shared.Models;
public class QuizDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string PinCode { get; set; } = null!;
    public List<QuestionDto> Questions { get; set; } = new();
}