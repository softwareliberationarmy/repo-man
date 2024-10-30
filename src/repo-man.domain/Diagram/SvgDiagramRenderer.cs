using Microsoft.Extensions.Configuration;
using repo_man.domain.Diagram.Legend;
using repo_man.domain.FileSystem;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram
{
    public class SvgDiagramRenderer : IDiagramRenderer
    {
        private readonly ISvgChartDataWriter _chartWriter;
        private readonly FileExtensionLegendDataBuilder _legendBuilder;
        private readonly ISvgLegendDataWriter _legendWriter;
        private readonly ISvgComposer _composer;
        private readonly IFileSystem _fileWriter;
        private readonly IConfiguration _config;

        public SvgDiagramRenderer(ISvgChartDataWriter chartWriter, ISvgLegendDataWriter legendWriter, ISvgComposer composer, IFileSystem fileWriter, IConfiguration config, FileExtensionLegendDataBuilder legendBuilder)
        {
            _chartWriter = chartWriter;
            _legendWriter = legendWriter;
            _composer = composer;
            _fileWriter = fileWriter;
            _config = config;
            _legendBuilder = legendBuilder;
        }

        public async Task CreateDiagram(GitTree tree)
        {
            var finalSvg = BuildSvgString(tree);

            var path = _config["outputDir"] ?? _config["repo"];

            var fileName = _config["fileName"] ?? "diagram.svg";
            await _fileWriter.WriteTextToFileAsync(finalSvg, Path.Combine(path!, fileName));
        }

        private string BuildSvgString(GitTree tree)
        {
            var chartData = _chartWriter.WriteChartData(tree);
            var legendDictionary = _legendBuilder.BuildLegendOptions(tree);
            var legendData = _legendWriter.WriteLegendData(legendDictionary, chartData.Size);

            var finalSvg = _composer.Compose(chartData, legendData);
            return finalSvg;
        }
    }
}
