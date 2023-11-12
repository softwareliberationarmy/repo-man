using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using repo_man.domain.DependencyInjection;
using repo_man.infrastructure.DependencyInjection;

namespace repo_man.console;

public static class Bootstrapper
{
    public static T InitializeToTopLevelService<T>(string[] args) where T : notnull
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDomainServices();
                services.AddInfrastructureServices();
            })
            .Build();

        return host.Services.GetRequiredService<T>();
    }

}