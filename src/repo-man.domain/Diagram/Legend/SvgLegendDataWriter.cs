using System.Drawing;
using System.Text;
using repo_man.domain.Diagram.FileColorMapper;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram.Legend;

public class SvgLegendDataWriter : ISvgLegendDataWriter
{
    private readonly FileExtensionLegendDataBuilder _fileExtensionLegendDataBuilder;

    public SvgLegendDataWriter(FileExtensionLegendDataBuilder builder)
    {
        _fileExtensionLegendDataBuilder = builder;
    }

    public LegendData WriteLegendData(GitTree tree, Point startingPoint)
    {
        var builder = new StringBuilder();
        builder.Append($"<g transform=\"translate({startingPoint.X}, {startingPoint.Y})\">");

        var extensions = _fileExtensionLegendDataBuilder.BuildLegendOptions(tree);

        var y = 0;
        foreach (var extension in extensions)
        {
            builder.Append($"<g transform=\"translate(0, {y})\">");
            builder.Append($"<circle r=\"5\" fill=\"{extension.Value}\"></circle>");
            builder.Append(
                $"<text x=\"10\" style=\"font-size:14px;font-weight:300\" dominant-baseline=\"middle\">{extension.Key}</text>");
            builder.Append("</g>");
            y += 15;
        }
        builder.Append("</g>");

        return new LegendData
        {
            Data = builder.ToString(),
            Size = new Point(100, y)
        };
    }
}