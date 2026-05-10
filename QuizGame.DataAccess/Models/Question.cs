using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Models;
public class Question
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("Quiz")]
    public int QuizId { get; set; }
    [Required]
    [MaxLength(255)]
    public string Text { get; set; } = null!;

    public int Order { get; set; }

    [Range(1, 600)]
    public int? TimeLimitSeconds { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual ICollection<AnswerOption> AnswerOptions { get; set; } = [];

    public virtual ICollection<Submission> Submissions { get; set; } = [];
}