using Microsoft.Extensions.DependencyInjection;

namespace QuizGame.SignalR;

public static class DependencyInjection
{
    public static IServiceCollection AddSignalRServices(
        this IServiceCollection services)
    {
        services.AddSignalR();

        return services;
    }
}