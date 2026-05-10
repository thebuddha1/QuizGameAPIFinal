namespace QuizGame.Shared.Models;

public class SubmitAnswerDto
{
    public int SessionId { get; set; }
    public int ParticipantId { get; set; }
    public int QuestionId { get; set; }
    public int AnswerOptionId { get; set; }
}