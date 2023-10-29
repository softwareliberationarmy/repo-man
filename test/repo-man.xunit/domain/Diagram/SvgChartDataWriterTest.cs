using AutoFixture;
using FluentAssertions;
using Moq.AutoMock;
using repo_man.domain.Diagram;
using repo_man.domain.Git;

namespace repo_man.xunit.domain.Diagram
{
    public class SvgChartDataWriterTest
    {
        private readonly AutoMocker _mocker;

        public SvgChartDataWriterTest()
        {
            _mocker = new AutoMocker();
        }

        [Theory]
        [InlineData("#0060ac", "Program.cs", ".cs")]
        [InlineData("green", "Program.cs", ".cs")]
        [InlineData("aliceblue", "Main.cs", ".cs")]
        [InlineData("#012345", "Readme.md", ".md")]
        [InlineData("black", ".gitignore", ".gitignore")]
        public void Single_CS_File(string color, string fileName, string fileExtension)
        {
            var fixture = new Fixture();

            GivenTheseColorMappings(new Tuple<string, string>(fileExtension, color));

            var tree = new GitTree();
            tree.AddFile(fileName, fixture.Create<long>(), Array.Empty<Commit>() );
            
            var target = _mocker.CreateInstance<SvgChartDataWriter>();

            var result = target.WriteChartData(tree);

            result.Data.Should().Be($"<g style=\"fill:{color}\" transform=\"translate(20,20)\">" +
                                    "<circle r=\"10\" />" +
                                    $"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>{fileName}</text>" +
                                    "</g>");

        }

        [Fact]
        public void Two_TopLevel_Files_Same_Size()
        {
            GivenTheseColorMappings(
                new Tuple<string, string>(".cs", "pink"),
                new Tuple<string, string>(".md", "blue")
            );

            var tree = new GitTree();
            tree.AddFile("Program.cs", 100, Array.Empty<Commit>());
            tree.AddFile("Readme.md", 100, Array.Empty<Commit>());

            var target = _mocker.CreateInstance<SvgChartDataWriter>();

            var result = target.WriteChartData(tree);

            result.Data.Should().Be("<g style=\"fill:pink\" transform=\"translate(20,20)\">" +
                                    "<circle r=\"10\" />" +
                                    "<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>Program.cs</text>" +
                                    "</g>" +
                                    "<g style=\"fill:blue\" transform=\"translate(40,20)\">" +
                                    "<circle r=\"10\" />" +
                                    "<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>Readme.md</text>" +
                                    "</g>");
        }

        private void GivenTheseColorMappings(params Tuple<string, string>[] mappings)
        {
            foreach (var mapping in mappings)
            {
                _mocker.GetMock<IFileColorMapper>().Setup(x => x.Map(mapping.Item1)).Returns(mapping.Item2);
            }
        }
    }
}
