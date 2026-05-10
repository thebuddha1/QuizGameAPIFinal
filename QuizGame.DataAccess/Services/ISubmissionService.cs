using QuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGame.DataAccess.Services;
public interface ISubmissionService
{
    Task<Submission> SubmitAnswerAsync(int participantId, int questionId, int answerOptionId);

    Task<List<Submission>> GetByQuestionAsync(int questionId);
    Task<List<Submission>> GetByParticipantAsync(int participantId);
    Task<Submission?> GetByParticipantAndQuestionAsync(int participantId, int questionId);
}