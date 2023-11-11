using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using repo_man.domain;
using repo_man.domain.Diagram;
using repo_man.domain.Git;
using repo_man.infrastructure;

namespace repo_man.console;

public static class Bootstrapper
{
    public static T InitializeToTopLevelService<T>(string[] args) where T : notnull
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTransient<IGitRepoCrawler, LibGit2SharpGitRepoCrawler>();
                services.AddTransient<ITreeExtracter, GitRepositoryTreeExtracter>();
                services.AddTransient<IDiagramRenderer, SvgDiagramRenderer>();
                services.AddTransient<SvgChartStringBuilder>();
                services.AddTransient<RepositoryVisualizer>();
            })
            .Build();

        return host.Services.GetRequiredService<T>();
    }
}