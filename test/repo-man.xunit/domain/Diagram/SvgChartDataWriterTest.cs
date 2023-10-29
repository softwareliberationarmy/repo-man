using System.Drawing;
using AutoFixture;
using FluentAssertions;
using Moq.AutoMock;
using repo_man.domain.Diagram;
using repo_man.domain.Git;

namespace repo_man.xunit.domain.Diagram
{
    public class SvgChartDataWriterTest
    {
        [Theory]
        [InlineData("#0060ac", "Program.cs")]
        [InlineData("green", "Program.cs")]
        [InlineData("aliceblue", "Main.cs")]
        public void Single_CS_File(string color, string fileName)
        {
            var fixture = new Fixture();
            var mocker = new AutoMocker();

            mocker.GetMock<IFileColorMapper>().Setup(x => x.Map(".cs")).Returns(color);

            var tree = new GitTree();
            tree.AddFile(fileName, fixture.Create<long>(), Array.Empty<Commit>() );
            
            var target = mocker.CreateInstance<SvgChartDataWriter>();

            var result = target.WriteChartData(tree);

            result.Data.Should().Be($"<g style=\"fill:{color}\" transform=\"translate(20,20)\">" +
                                    "<circle r=\"10\" />" +
                                    $"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>{fileName}</text>" +
                                    "</g>");
        }
    }

    public class SvgChartDataWriter
    {
        public readonly IFileColorMapper _colorMapper;

        public SvgChartDataWriter(IFileColorMapper colorMapper)
        {
            _colorMapper = colorMapper;
        }
        
        public ChartData WriteChartData(GitTree tree)
        {
            var firstFile = tree.Files.Single();
            var color = _colorMapper.Map(Path.GetExtension(firstFile.Name));

            var chartData = new ChartData
            {
                Data = $"<g style=\"fill:{color}\" transform=\"translate(20,20)\">" +
                       "<circle r=\"10\" />" +
                       $"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>{firstFile.Name}</text>" +
                       "</g>"
            };

            return chartData;
        }
    }
}
