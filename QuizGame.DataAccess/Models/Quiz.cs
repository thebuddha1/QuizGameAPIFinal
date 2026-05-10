using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuizGame.DataAccess.Models;
public class Quiz
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = null!;

    [Required]
    [ForeignKey("User")]
    public string UserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsPublished { get; set; }
    [Required]
    [MaxLength(10)]
    public string PinCode { get; set; } = null!;

    public QuizState Status { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = [];

    public virtual ICollection<QuizSession> Sessions { get; set; } = [];
}