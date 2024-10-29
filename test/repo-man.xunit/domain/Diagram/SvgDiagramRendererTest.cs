using System.Drawing;
using AutoFixture;
using Microsoft.Extensions.Configuration;
using Moq;
using repo_man.domain.Diagram.Legend;
using repo_man.domain.FileSystem;

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
            _mocker.GetMock<IConfiguration>().Setup(c => c["outputDir"]).Returns("C:\\temp");
            _mocker.GetMock<ISvgChartDataWriter>().Setup(x => x.WriteChartData(tree))
                .Returns(chartData);
            _mocker.GetMock<ISvgLegendDataWriter>().Setup(x => x.WriteLegendData(tree, point))
                .Returns(legendData);
            _mocker.GetMock<ISvgComposer>().Setup(x => x.Compose(chartData, legendData))
                .Returns(expectedSvg);
            _mocker.GetMock<IFileSystem>().Setup(x => x.WriteTextToFileAsync(expectedSvg, It.IsAny<string>()));

            await WhenICreateADiagramFrom(tree);

            _mocker.VerifyAll();
        }

        [Fact]
        public async Task Uses_Path_From_Config_And_Default_File_Name()
        {
            _mocker.GetMock<IConfiguration>().Setup(c => c["outputDir"]).Returns("C:\\temp");
            _mocker.GetMock<ISvgChartDataWriter>().Setup(x => x.WriteChartData(It.IsAny<GitTree>()))
                .Returns(_fixture.Build<ChartData>().With(x => x.Size, Point.Empty).Create());
            _mocker.GetMock<ISvgLegendDataWriter>().Setup(x => x.WriteLegendData(It.IsAny<GitTree>(), It.IsAny<Point>()))
                .Returns(_fixture.Build<LegendData>().With(x => x.Size, Point.Empty).Create());
            _mocker.GetMock<ISvgComposer>().Setup(x => x.Compose(It.IsAny<ChartData>(), It.IsAny<LegendData>()))
                .Returns(_fixture.Create<string>());
            _mocker.GetMock<IFileSystem>().Setup(x => x.WriteTextToFileAsync(It.IsAny<string>(), "C:\\temp\\diagram.svg"));

            await WhenICreateADiagramFrom(new GitTree());

            _mocker.VerifyAll();
        }

        [Fact]
        public async Task Can_Use_Path_And_File_Name_From_Config()
        {
            _mocker.GetMock<IConfiguration>().Setup(c => c["outputDir"]).Returns("C:\\temp");
            _mocker.GetMock<IConfiguration>().Setup(c => c["fileName"]).Returns("MyDiagram.svg");
            _mocker.GetMock<ISvgChartDataWriter>().Setup(x => x.WriteChartData(It.IsAny<GitTree>()))
                .Returns(_fixture.Build<ChartData>().With(x => x.Size, Point.Empty).Create());
            _mocker.GetMock<ISvgLegendDataWriter>().Setup(x => x.WriteLegendData(It.IsAny<GitTree>(), It.IsAny<Point>()))
                .Returns(_fixture.Build<LegendData>().With(x => x.Size, Point.Empty).Create());
            _mocker.GetMock<ISvgComposer>().Setup(x => x.Compose(It.IsAny<ChartData>(), It.IsAny<LegendData>()))
                .Returns(_fixture.Create<string>());
            _mocker.GetMock<IFileSystem>().Setup(x => x.WriteTextToFileAsync(It.IsAny<string>(), "C:\\temp\\MyDiagram.svg"));

            await WhenICreateADiagramFrom(new GitTree());

            _mocker.VerifyAll();
        }

        [Fact]
        public async Task Uses_Repo_If_No_Path_From_Config()
        {
            _mocker.GetMock<IConfiguration>().Setup(c => c["repo"]).Returns("C:\\temp2");
            _mocker.GetMock<ISvgChartDataWriter>().Setup(x => x.WriteChartData(It.IsAny<GitTree>()))
                .Returns(_fixture.Build<ChartData>().With(x => x.Size, Point.Empty).Create());
            _mocker.GetMock<ISvgLegendDataWriter>().Setup(x => x.WriteLegendData(It.IsAny<GitTree>(), It.IsAny<Point>()))
                .Returns(_fixture.Build<LegendData>().With(x => x.Size, Point.Empty).Create());
            _mocker.GetMock<ISvgComposer>().Setup(x => x.Compose(It.IsAny<ChartData>(), It.IsAny<LegendData>()))
                .Returns(_fixture.Create<string>());
            _mocker.GetMock<IFileSystem>().Setup(x => x.WriteTextToFileAsync(It.IsAny<string>(), "C:\\temp2\\diagram.svg"));

            await WhenICreateADiagramFrom(new GitTree());

            _mocker.VerifyAll();
        }
        
        private async Task WhenICreateADiagramFrom(GitTree tree)
        {
            var target = _mocker.CreateInstance<SvgDiagramRenderer>();
            await target.CreateDiagram(tree);
        }
    }
}
