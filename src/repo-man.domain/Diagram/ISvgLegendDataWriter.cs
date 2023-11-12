using System.Drawing;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public interface ISvgLegendDataWriter
{
    LegendData WriteLegendData(GitTree tree, Point startingPoint);
}