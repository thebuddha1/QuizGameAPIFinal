using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Models;
public class AnswerOption
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Question")]
    public int QuestionId { get; set; }

    [Required]
    [MaxLength(300)]
    public string Text { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual ICollection<Submission> Submissions { get; set; } = [];
}