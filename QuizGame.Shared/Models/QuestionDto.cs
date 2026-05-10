using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.Shared.Models;
public class QuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public int Order { get; set; }
    public int TimeLimitSeconds { get; set; }
    public List<AnswerOptionDto> Answers { get; set; } = new();
}