using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Models;
public class Submission
{
    [Key]
    public int Id { get; set; }
    public int ParticipantId { get; set; }
    public int QuestionId { get; set; }
    public int AnswerOptionId { get; set; }

    public DateTime SubmittedAt { get; set; }

    public bool IsCorrect { get; set; }

    public virtual Participant Participant { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;

    public virtual AnswerOption AnswerOption { get; set; } = null!;
}