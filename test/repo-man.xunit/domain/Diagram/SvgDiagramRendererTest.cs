using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using repo_man.domain.Diagram;
using repo_man.domain.Git;
using repo_man.xunit._helpers;

namespace repo_man.xunit.domain.Diagram
{
    public class SvgDiagramRendererTest: TestBase
    {
        [Fact]
        public async Task Composes_Svg()
        {
            var tree = new GitTree();
            var point = new Point(123, 456);
            var chartData = new ChartData{ Data = "", Size = point};
            var legendData = new LegendData { Data = "", Size = new Point(100, 30) };
            var expectedSvg = "Test";
            _mocker.GetMock<ISvgChartDataWriter>().Setup(x => x.WriteChartData(tree))
                .Returns(chartData);
            _mocker.GetMock<ISvgLegendDataWriter>().Setup(x => x.WriteLegendData(tree, point))
                .Returns(legendData);
            _mocker.GetMock<ISvgComposer>().Setup(x => x.Compose(chartData, legendData))
                .Returns(expectedSvg);

            var target = _mocker.CreateInstance<SvgDiagramRenderer>();

            await target.CreateDiagram(tree);

            _mocker.Verify<ISvgChartDataWriter>(x => x.WriteChartData(tree), Times.Once);
            _mocker.Verify<ISvgLegendDataWriter>(x => x.WriteLegendData(tree, point), Times.Once);
            _mocker.Verify<ISvgComposer>(x => x.Compose(chartData, legendData), Times.Once);
            _mocker.Verify<IFileWriter>(x => x.WriteTextToFile(expectedSvg, It.IsAny<string>()), Times.Once);

        }
    }
}
