using System.Runtime.CompilerServices;
using repo_man.domain.Git;


namespace repo_man.domain.Diagram
{
    public class SvgDiagramRenderer : IDiagramRenderer
    {
        private readonly ISvgChartDataWriter _chartWriter;
        private readonly ISvgLegendDataWriter _legendWriter;
        private readonly ISvgComposer _composer;
        private readonly IFileWriter _fileWriter;

        public SvgDiagramRenderer(ISvgChartDataWriter chartWriter, ISvgLegendDataWriter legendWriter, ISvgComposer composer, IFileWriter fileWriter)
        {
            _chartWriter = chartWriter;
            _legendWriter = legendWriter;
            _composer = composer;
            _fileWriter = fileWriter;
        }

        public async Task CreateDiagram(GitTree tree)
        {
            var chartData = _chartWriter.WriteChartData(tree);
            var legendData = _legendWriter.WriteLegendData(tree, chartData.Size);

            var finalSvg = _composer.Compose(chartData, legendData);

            await _fileWriter.WriteTextToFile(finalSvg, "Diagram.svg");

            //now we have the full svg size, so add the header and footer
            //now write it to a file
        }
    }
}
