using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Models;
public class Participant
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("QuizSession")]
    public int QuizSessionId { get; set; }

    [ForeignKey("User")]  
    public string? UserId { get; set; }
    public virtual User? User { get; set; }

    [MaxLength(100)]
    public string? Nickname { get; set; }

    public DateTime JoinedAt { get; set; }

    public virtual QuizSession QuizSession { get; set; } = null!;

    public virtual ICollection<Submission> Submissions { get; set; } = [];
}