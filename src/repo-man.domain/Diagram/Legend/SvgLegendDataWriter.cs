using System.Drawing;
using System.Text;

namespace repo_man.domain.Diagram.Legend;

public class SvgLegendDataWriter : ISvgLegendDataWriter
{
    public LegendData WriteLegendData(Dictionary<string, string> extensions, Point startingPoint)
    {
        var builder = new StringBuilder();
        builder.Append($"<g transform=\"translate({startingPoint.X + 20}, {startingPoint.Y - (15 * extensions.Count)})\">");

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