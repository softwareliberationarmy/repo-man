using repo_man.domain.Diagram.Legend;

namespace repo_man.domain.Diagram;

public interface ISvgComposer
{
    string Compose(ChartData chartData, LegendData legendData);
}