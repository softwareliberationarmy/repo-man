using Microsoft.Extensions.DependencyInjection;
using repo_man.domain.AI;
using repo_man.domain.CodeQuality;
using repo_man.domain.Diagram;
using repo_man.domain.Diagram.Calculators;
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
            //git
            services.AddTransient<ITreeExtracter, GitRepositoryTreeExtracter>();

            //diagrams and svg
            services.AddSingleton<IDiagramRenderer, SvgDiagramRenderer>();
            services.AddSingleton<ISvgChartDataWriter, SvgBoxChartDataWriter>();
            services.AddSingleton<ISvgLegendDataWriter, SvgLegendDataWriter>();
            services.AddSingleton<IFileColorMapper, ConstantFileColorMapper>();
            services.AddSingleton<ISvgComposer, SvgFrugalComposer>();
            services.AddSingleton<IFileRadiusCalculator, BoundedFileRadiusCalculator>();
            services.AddSingleton<SvgChartStringBuilder>();
            services.AddSingleton<RepositoryVisualizer>();
            services.AddSingleton<FileExtensionLegendDataBuilder>();

            //code quality and ai
            services.AddSingleton<ICodeQualityAnalyst, LlmCodeQualityAnalyst>();
            services.AddSingleton<IRagDataBuilder, JsonRagDataBuilder>();
            services.AddSingleton<RepositoryReviewer>();
            services.AddTransient<BoundedIntCalculator>();
            services.AddTransient<IRiskIndexer, SonarQubeRiskIndexer>();
            services.AddTransient<RiskIndexCalculator<SonarQubeCodeQualityData>>();

            //the cheese stands alone
            services.AddSingleton<RepoMan>();

            return services;
        }

    }
}
