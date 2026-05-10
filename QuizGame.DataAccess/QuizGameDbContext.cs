using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizGame.DataAccess.Models;

namespace QuizGame.DataAccess;

public class QuizGameDbContext : IdentityDbContext<User, UserRole, string>
{
    public DbSet<Quiz> Quizzes { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<QuizSession> QuizSessions { get; set; } = null!;
    public DbSet<Participant> Participants { get; set; } = null!;
    public DbSet<Submission> Submissions { get; set; } = null!;
    public DbSet<AnswerOption> AnswersOptions { get; set; } = null!;

    public QuizGameDbContext(DbContextOptions<QuizGameDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnswerOption>()
            .HasOne(a => a.Question)
            .WithMany(q => q.AnswerOptions)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuizSession>()
            .HasOne(s => s.Quiz)
            .WithMany(q => q.Sessions)
            .HasForeignKey(s => s.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Participant>()
            .HasOne(p => p.QuizSession)
            .WithMany(s => s.Participants)
            .HasForeignKey(p => p.QuizSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Participant)
            .WithMany(p => p.Submissions)
            .HasForeignKey(s => s.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Submission>()
            .HasOne(s => s.Question)
            .WithMany(q => q.Submissions)
            .HasForeignKey(s => s.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Submission>()
            .HasOne(s => s.AnswerOption)
            .WithMany(a => a.Submissions)
            .HasForeignKey(s => s.AnswerOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<QuizSession>()
            .HasOne(s => s.CurrentQuestion)
            .WithMany()
            .HasForeignKey(s => s.CurrentQuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Participant>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Submission>()
            .HasIndex(s => new { s.ParticipantId, s.QuestionId })
            .IsUnique();
    }
}