using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizGame.DataAccess.Models;

namespace QuizGame.DataAccess;

public static class DbInitializer
{
    public static async Task Initialize(
    QuizGameDbContext context,
    UserManager<User> userManager)
    {
        await context.Database.MigrateAsync();

        if ( await context.Quizzes.AnyAsync())
           return;

        var users = new List<User>();

        for (int i = 1; i <= 3; i++)
        {
            var email = $"user{i}@test.com";

            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser == null)
            {
                var user = new User
                {
                    UserName = email,
                    Email = email,
                    Name = $"USER{i}"
                };

                var result = await userManager.CreateAsync(user, "Test123@");

                if (result.Succeeded)
                    users.Add(user);
            }
            else
            {
                users.Add(existingUser);
            }
        }

        var quizzes = new List<Quiz>();

        for (int i = 1; i <= 5; i++)
        {
            var user = users[(i - 1) % users.Count];

            var quiz = new Quiz
            {
                Title = $"Quiz {i}",
                CreatedAt = DateTime.UtcNow,
                PinCode = $"{100000 + i}",
                UserId = user.Id,
                Status = QuizState.Draft,
                Questions = new List<Question>()
            };

            for (int q = 1; q <= 2; q++)
            {
                var correctIndex = Random.Shared.Next(0, 2);

                var question = new Question
                {
                    Text = $"Question {q} of Quiz {i}",
                    Order = q - 1,
                    TimeLimitSeconds = 30,
                    AnswerOptions = new List<AnswerOption>()
                };

                for (int a = 0; a < 2; a++)
                {
                    question.AnswerOptions.Add(new AnswerOption
                    {
                        Text = $"Answer {a + 1}",
                        IsCorrect = a == correctIndex
                    });
                }

                quiz.Questions.Add(question);
            }

            quizzes.Add(quiz);
        }

        context.Quizzes.AddRange(quizzes);
        await context.SaveChangesAsync();
    }
}