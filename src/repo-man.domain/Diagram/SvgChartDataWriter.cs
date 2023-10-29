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

        var minFileSize = tree.GetMinFileSize();

        long x = 10;
        for (int i = 0; i < tree.Files.Count; i++)
        {
            var file = tree.Files.ElementAt(i);
            var color = _colorMapper.Map(Path.GetExtension(file.Name));
            var radius = (file.FileSize / minFileSize) * 10;
            x += radius;
            long y = 10 + radius;

            chartData.Data += $"<g style=\"fill:{color}\" transform=\"translate({x},{y})\">" +
                              $"<circle r=\"{radius}\" />" +
                              $"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>{file.Name}</text>" +
                              "</g>";
            x += radius;  
        }

        return chartData;
    }
}