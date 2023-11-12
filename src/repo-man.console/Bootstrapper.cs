using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using repo_man.domain;
using repo_man.domain.Diagram;
using repo_man.domain.Git;
using repo_man.infrastructure.FileSys;
using repo_man.infrastructure.GitFileSys;

namespace repo_man.console;

public static class Bootstrapper
{
    public static T InitializeToTopLevelService<T>(string[] args) where T : notnull
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<ITreeExtracter, GitRepositoryTreeExtracter>();
                services.AddTransient<IDiagramRenderer, SvgDiagramRenderer>();
                services.AddTransient<ISvgChartDataWriter, SvgBoxChartDataWriter>();
                services.AddTransient<ISvgLegendDataWriter, SvgLegendDataWriter>();
                services.AddTransient<IFileColorMapper, ConstantFileColorMapper>();
                services.AddTransient<ISvgComposer, SvgFrugalComposer>();
                services.AddTransient<SvgChartStringBuilder>();
                services.AddTransient<RepositoryVisualizer>();

                services.AddInfrastructureServices();
            })
            .Build();

        return host.Services.GetRequiredService<T>();
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<IGitRepoCrawler, LibGit2SharpGitRepoCrawler>();
        services.AddTransient<IFileWriter, WindowsFileWriter>();
        return services;
    }
}