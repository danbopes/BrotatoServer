using BrotatoServer.Controllers;
using BrotatoServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrotatoServer.Tests;

public class DITools
{
    public static IServiceProvider CreateDefaultServiceProvider(Action<IServiceCollection>? additionalConfiguring = null)
    {
        var services = new ServiceCollection();

        services
            .AddDbContext<BrotatoServerContext>(options => options
                .UseInMemoryDatabase($"{Guid.NewGuid()}"))
            .AddLogging(o => o.AddConsole())
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IRunRepository, RunRepository>()
            .AddScoped<UsersController>()
            .AddScoped<SetupController>()
            .AddScoped<RunsController>();

        additionalConfiguring?.Invoke(services);

        return services.BuildServiceProvider();
    }
}