using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizGame.DataAccess;

namespace QuizGame.Tests.Infrastructure;

public class TestWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public TestWebApplicationFactory()
    {
        _connection =
            new SqliteConnection(
                "DataSource=:memory:");

        _connection.Open();
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var dbDescriptor =
                services.SingleOrDefault(
                    d =>
                        d.ServiceType ==
                        typeof(
                            DbContextOptions<QuizGameDbContext>));

            if (dbDescriptor != null)
            {
                services.Remove(dbDescriptor);
            }

            services.AddDbContext<QuizGameDbContext>(
                options =>
                {
                    options.UseSqlite(_connection);
                });

            var sp =
                services.BuildServiceProvider();

            using var scope =
                sp.CreateScope();

            var db =
                scope.ServiceProvider
                    .GetRequiredService<QuizGameDbContext>();

            db.Database.EnsureDeleted();

            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _connection.Dispose();
    }
}