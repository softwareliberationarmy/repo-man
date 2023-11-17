using Microsoft.Extensions.Configuration;
using repo_man.domain.Diagram.Legend;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram
{
    public class SvgDiagramRenderer : IDiagramRenderer
    {
        private readonly ISvgChartDataWriter _chartWriter;
        private readonly ISvgLegendDataWriter _legendWriter;
        private readonly ISvgComposer _composer;
        private readonly IFileWriter _fileWriter;
        private readonly IConfiguration _config;

        public SvgDiagramRenderer(ISvgChartDataWriter chartWriter, ISvgLegendDataWriter legendWriter, ISvgComposer composer, IFileWriter fileWriter, IConfiguration config)
        {
            _chartWriter = chartWriter;
            _legendWriter = legendWriter;
            _composer = composer;
            _fileWriter = fileWriter;
            _config = config;
        }

        public async Task CreateDiagram(GitTree tree)
        {
            var finalSvg = BuildSvgString(tree);

            var path = _config["outputDir"] ?? _config["repo"];

            await _fileWriter.WriteTextToFile(finalSvg, Path.Combine(path!, "diagram.svg"));
        }

        private string BuildSvgString(GitTree tree)
        {
            var chartData = _chartWriter.WriteChartData(tree);
            var legendData = _legendWriter.WriteLegendData(tree, chartData.Size);

            var finalSvg = _composer.Compose(chartData, legendData);
            return finalSvg;
        }
    }
}
