using Microsoft.Extensions.DependencyInjection;
using repo_man.domain.Diagram;
using repo_man.domain.Diagram.FileColorMapper;
using repo_man.domain.Diagram.FileRadiusCalculator;
using repo_man.domain.Diagram.Legend;
using repo_man.domain.Git;

namespace repo_man.domain.DependencyInjection
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<ITreeExtracter, GitRepositoryTreeExtracter>();
            services.AddScoped<IDiagramRenderer, SvgDiagramRenderer>();
            services.AddScoped<ISvgChartDataWriter, SvgBoxChartDataWriter>();
            services.AddScoped<ISvgLegendDataWriter, SvgLegendDataWriter>();
            services.AddScoped<IFileColorMapper, ConstantFileColorMapper>();
            services.AddScoped<ISvgComposer, SvgFrugalComposer>();
            services.AddScoped<IFileRadiusCalculator, BoundedFileRadiusCalculator>();
            services.AddScoped<SvgChartStringBuilder>();
            services.AddScoped<RepositoryVisualizer>();

            return services;
        }

    }
}
