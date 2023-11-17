using System.Drawing;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram.Legend;

public interface ISvgLegendDataWriter
{
    LegendData WriteLegendData(GitTree tree, Point startingPoint);
}