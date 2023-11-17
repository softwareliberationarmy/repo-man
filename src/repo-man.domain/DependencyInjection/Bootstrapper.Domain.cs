using Microsoft.Extensions.DependencyInjection;
using repo_man.domain.Diagram;
using repo_man.domain.Diagram.FileRadiusCalculator;
using repo_man.domain.Diagram.Legend;
using repo_man.domain.Git;

namespace repo_man.domain.DependencyInjection
{
    public static class Bootstrapper
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddTransient<ITreeExtracter, GitRepositoryTreeExtracter>();
            services.AddTransient<IDiagramRenderer, SvgDiagramRenderer>();
            services.AddTransient<ISvgChartDataWriter, SvgBoxChartDataWriter>();
            services.AddTransient<ISvgLegendDataWriter, SvgLegendDataWriter>();
            services.AddTransient<IFileColorMapper, ConstantFileColorMapper>();
            services.AddTransient<ISvgComposer, SvgFrugalComposer>();
            services.AddTransient<IFileRadiusCalculator, UnboundedFileRadiusCalculator>();
            services.AddTransient<SvgChartStringBuilder>();
            services.AddTransient<RepositoryVisualizer>();

            return services;
        }

    }
}
