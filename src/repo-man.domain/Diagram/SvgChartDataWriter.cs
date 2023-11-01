using repo_man.domain.Git;
using System.Drawing;
using System.Text;

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
        var builder = new StringBuilder();

        var minFileSize = tree.GetMinFileSize();

        long x = 10;
        long startY = 0;
        long maxY = 0;
        for (int i = 0; i < tree.Files.Count; i++)
        {
            startY = 10;
            var file = tree.Files.ElementAt(i);
            var color = _colorMapper.Map(Path.GetExtension(file.Name));
            var radius = (file.FileSize / minFileSize) * 10;
            x += radius;
            var y = startY + radius;

            builder.Append($"<g style=\"fill:{color}\" transform=\"translate({x},{y})\">");
            builder.Append($"<circle r=\"{radius}\" />");
            builder.Append($"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>{file.Name}</text>");
            builder.Append("</g>");
            x += radius;
            maxY = Math.Max(maxY, y + radius + 10);

        }

        startY = maxY;
        x = 15;

        for (int i = 0; i < tree.Folders.Count; i++)
        {
            startY += 10;
            var folder = tree.Folders.ElementAt(i);
            for (int j = 0; j < folder.Files.Count; j++)
            {
                var file = folder.Files.ElementAt(j);
                var color = _colorMapper.Map(Path.GetExtension(file.Name));
                var radius = (file.FileSize / minFileSize) * 10;
                x += radius;
                long y = startY + 5 + radius;

                builder.Append($"<g style=\"fill:{color}\" transform=\"translate({x},{y})\">");
                builder.Append($"<circle r=\"{radius}\" />");
                builder.Append($"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>{file.Name}</text>");
                builder.Append("</g>");
                x += radius;
                maxY = Math.Max(maxY, y + radius + 10);
            }

            builder.Append($"<g transform=\"translate(10,{startY})\">");
            builder.Append($"<rect fill=\"none\" stroke-width=\"0.5\" stroke=\"black\" width=\"{x - 5}\" height=\"{maxY - 15}\" />");
            builder.Append($"<text style=\"fill:black\" font-size=\"6\" transform=\"translate(-1,-1)\" >{folder.Name}</text>");
            builder.Append("</g>");

        }

        return new ChartData { Data = builder.ToString() };
    }
}