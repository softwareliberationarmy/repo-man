using Microsoft.Extensions.DependencyInjection;
using repo_man.domain.AI;
using repo_man.domain.FileSystem;
using repo_man.domain.Git;
using repo_man.infrastructure.AI;
using repo_man.infrastructure.FileSys;
using repo_man.infrastructure.Git;

namespace repo_man.infrastructure.DependencyInjection
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddTransient<IGitRepoCrawler, ConsoleLogGitRepoCrawler>();
            services.AddTransient<IFileSystem, WindowsFileSystem>();
            services.AddTransient<ITextGenerationModel, OllamaTextGenerationModel>();
            services.AddTransient<GitLogParser>();
            return services;
        }
    }
}
