using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class SvgChartDataWriter
{
    public readonly IFileColorMapper _colorMapper;

    public SvgChartDataWriter(IFileColorMapper colorMapper)
    {
        _colorMapper = colorMapper;
    }
        
    public ChartData WriteChartData(GitTree tree)
    {
        var chartData = new ChartData { Data = "" };

        for (int i = 0; i < tree.Files.Count; i++)
        {
            var file = tree.Files.ElementAt(i);
            var color = _colorMapper.Map(Path.GetExtension(file.Name));
            var x = 20 + (i * 20);

            chartData.Data += $"<g style=\"fill:{color}\" transform=\"translate({x},20)\">" +
                              "<circle r=\"10\" />" +
                              $"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>{file.Name}</text>" +
                              "</g>";
        }

        return chartData;
    }
}