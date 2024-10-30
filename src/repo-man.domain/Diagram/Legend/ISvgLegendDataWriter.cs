using System.Drawing;

namespace repo_man.domain.Diagram.Legend;

public interface ISvgLegendDataWriter
{
    LegendData WriteLegendData(Dictionary<string, string> tree, Point startingPoint);
}