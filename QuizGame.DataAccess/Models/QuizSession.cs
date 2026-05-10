using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizGame.DataAccess.Models;
public class QuizSession
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Quiz")]
    public int QuizId { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    [ForeignKey("Question")]
    public int? CurrentQuestionId { get; set; }

    public bool IsQuestionLocked { get; set; }

    public QuizSessionState State { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual Question? CurrentQuestion { get; set; }

    public virtual ICollection<Participant> Participants { get; set; } = [];
}