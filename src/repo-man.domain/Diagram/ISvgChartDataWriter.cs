using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public interface ISvgChartDataWriter
{
    ChartData WriteChartData(GitTree tree);
}