using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public interface ISvgBoxChartDataWriter
{
    ChartData WriteChartData(GitTree tree);
}