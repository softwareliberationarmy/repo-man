using AutoFixture;
using FluentAssertions;
using Moq.AutoMock;
using repo_man.domain.Diagram;
using repo_man.domain.Git;
using System.Drawing;

namespace repo_man.xunit.domain.Diagram
{
    public class SvgChartDataWriterTest
    {
        private readonly AutoMocker _mocker;
        private readonly Fixture _fixture;

        public SvgChartDataWriterTest()
        {
            _mocker = new AutoMocker();
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData("#0060ac", "Program.cs", ".cs")]
        [InlineData("green", "Program.cs", ".cs")]
        [InlineData("aliceblue", "Main.cs", ".cs")]
        [InlineData("#012345", "Readme.md", ".md")]
        [InlineData("black", ".gitignore", ".gitignore")]
        public void Single_TopLevel_CS_File(string color, string fileName, string fileExtension)
        {

            GivenTheseColorMappings(new Tuple<string, string>(fileExtension, color));
            
            var tree = GivenThisFileTree(
                new Tuple<string, long>(fileName, _fixture.Create<long>()));

            var result = WhenICreateChartData(tree);

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

            var tree = GivenThisFileTree(
                new Tuple<string, long>("Program.cs", 100),
                new Tuple<string, long>("Readme.md", 100)
            );

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be("<g style=\"fill:pink\" transform=\"translate(20,20)\">" +
                                    "<circle r=\"10\" />" +
                                    "<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>Program.cs</text>" +
                                    "</g>" +
                                    "<g style=\"fill:blue\" transform=\"translate(45,20)\">" +
                                    "<circle r=\"10\" />" +
                                    "<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>Readme.md</text>" +
                                    "</g>");
        }

        [Fact]
        public void Two_TopLevel_Files_Different_Sizes()
        {
            GivenTheseColorMappings(new Tuple<string, string>(".cs", "#001122"));
            var tree = GivenThisFileTree(
                new Tuple<string, long>("Program.cs", 100),
                new Tuple<string, long>("App.cs", 50));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be("<g style=\"fill:#001122\" transform=\"translate(30,30)\">" +
                                    "<circle r=\"20\" />" +
                                    "<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>Program.cs</text>" +
                                    "</g>" +
                                    "<g style=\"fill:#001122\" transform=\"translate(65,20)\">" +
                                    "<circle r=\"10\" />" +
                                    "<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>App.cs</text>" +
                                    "</g>");
        }

        [Fact]
        public void Single_File_In_A_Folder()
        {
            GivenTheseColorMappings(new Tuple<string, string>(".cs", "fuschia"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/Program.cs", _fixture.Create<long>()));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be($"<g style=\"fill:fuschia\" transform=\"translate(25,25)\">" +
                                    "<circle r=\"10\" />" +
                                    $"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>Program.cs</text>" +
                                    "</g>" +
                                    $"<g transform=\"translate(10,10)\">" +
                                    "<rect fill=\"none\" stroke-width=\"0.5\" stroke=\"black\" width=\"30\" height=\"30\" />" +
                                    $"<text style=\"fill:black\" font-size=\"6\" transform=\"translate(-1,-1)\" >src</text>" +
                                    "</g>");
        }

        [Fact]
        public void Two_Files_In_A_Folder_Same_Size()
        {
            GivenTheseColorMappings(new Tuple<string, string>(".cs", "fuschia"), new Tuple<string, string>(".xaml", "#001122"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/Program.cs", 100L),
                new Tuple<string, long>("src/App.xaml", 100L));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be($"<g style=\"fill:fuschia\" transform=\"translate(25,25)\">" +
                                    "<circle r=\"10\" />" +
                                    $"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>Program.cs</text>" +
                                    "</g>" +
                                    $"<g style=\"fill:#001122\" transform=\"translate(50,25)\">" +
                                    "<circle r=\"10\" />" +
                                    $"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>App.xaml</text>" +
                                    "</g>" +
                                    $"<g transform=\"translate(10,10)\">" +
                                    "<rect fill=\"none\" stroke-width=\"0.5\" stroke=\"black\" width=\"55\" height=\"30\" />" +
                                    $"<text style=\"fill:black\" font-size=\"6\" transform=\"translate(-1,-1)\" >src</text>" +
                                    "</g>");
        }

        private ChartData WhenICreateChartData(GitTree tree)
        {
            var target = _mocker.CreateInstance<SvgChartDataWriter>();
            var result = target.WriteChartData(tree);
            return result;
        }

        private static GitTree GivenThisFileTree(params Tuple<string, long>[] files)
        {
            var tree = new GitTree();
            foreach (var file in files)
            {
                tree.AddFile(file.Item1, file.Item2, Array.Empty<Commit>());
            }

            return tree;
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
