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

            result.Data.Should().Be(AFilledCircle(color, fileName, 20, 20, 10));
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

            result.Data.Should().Be(AFilledCircle("pink", "Program.cs", 20, 20, 10) +
                                    AFilledCircle("blue", "Readme.md", 45, 20, 10));
        }

        [Fact]
        public void Two_TopLevel_Files_Different_Sizes()
        {
            GivenTheseColorMappings(new Tuple<string, string>(".cs", "#001122"));
            var tree = GivenThisFileTree(
                new Tuple<string, long>("Program.cs", 100),
                new Tuple<string, long>("App.cs", 50));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(
                AFilledCircle("#001122", "Program.cs", 30, 30, 20) +
                AFilledCircle("#001122", "App.cs", 65, 20, 10));
        }

        [Fact]
        public void TopLevel_Files_Are_Ordered_By_Size_Descending()
        {
            GivenTheseColorMappings(new Tuple<string, string>(".cs", "#001122"));
            var tree = GivenThisFileTree(
                new Tuple<string, long>("App.cs", 50),
                new Tuple<string, long>("Program.cs", 100));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle("#001122", "Program.cs", 30, 30, 20) +
                                    AFilledCircle("#001122", "App.cs", 65, 20, 10));
        }

        [Fact]
        public void Single_File_In_A_Folder()
        {
            GivenTheseColorMappings(new Tuple<string, string>(".cs", "fuschia"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/Program.cs", _fixture.Create<long>()));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle("fuschia", "Program.cs", 25, 25, 10) +
                                    ARectangle(10, 10, 30, 30, "src"));
        }

        [Fact]
        public void Two_Files_In_A_Folder_Same_Size()
        {
            GivenTheseColorMappings(new Tuple<string, string>(".cs", "fuschia"), new Tuple<string, string>(".xaml", "#001122"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/Program.cs", 100L),
                new Tuple<string, long>("src/App.xaml", 100L));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle("fuschia", "Program.cs", 25, 25, 10) +
                                    AFilledCircle("#001122", "App.xaml", 50, 25, 10) +
                                    ARectangle(10, 10, 55, 30, "src"));
        }

        [Fact]
        public void Two_Files_In_A_Folder_Different_Sizes_Ordered_Descending()
        {
            GivenTheseColorMappings(
                new Tuple<string, string>(".cs", "fuschia"),
                new Tuple<string, string>(".xaml", "#001122"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/Program.cs", 100L),
                new Tuple<string, long>("src/App.xaml", 200L));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle("#001122", "App.xaml", 35, 35, 20) +
                                    AFilledCircle("fuschia", "Program.cs", 70, 25, 10) +
                                    ARectangle(10, 10, 75, 50, "src"));
        }

        [Fact]
        public void Two_Folders_Different_Sizes_Stacked()
        {
            GivenTheseColorMappings(
                new Tuple<string, string>(".cs", "blue"),
                new Tuple<string, string>(".md", "pink"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/Program.cs", 100L),
                new Tuple<string, long>("src/Bootstrapper.cs", 300L),
                new Tuple<string, long>("docs/About.md", 200L),
                new Tuple<string, long>("docs/GettingStarted.md", 400L));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle("blue", "Bootstrapper.cs", 45, 45, 30) +
                                    AFilledCircle("blue", "Program.cs", 90, 25, 10) +
                                    ARectangle(10, 10, 95, 70, "src") +
                                    AFilledCircle("pink", "GettingStarted.md", 55, 135, 40) +
                                    AFilledCircle("pink", "About.md", 120, 115, 20) +
                                    ARectangle(10, 90, 135, 90, "docs"));
        }

        //TO TEST: 
        // two folders stacked
        // two top-level files different sizes, plus two files in a folder, different sizes
        // nested folders

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

        private static string AFilledCircle(string color, string fileName, long expectedX, long expectedY, long expectedRadius)
        {
            return $"<g style=\"fill:{color}\" transform=\"translate({expectedX},{expectedY})\">" +
                   $"<circle r=\"{expectedRadius}\" />" +
                   $"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>{fileName}</text>" +
                   "</g>";
        }

        private string ARectangle(long expectedX, long expectedY, long width, long height, string text)
        {
            return $"<g transform=\"translate({expectedX},{expectedY})\">" +
                   $"<rect fill=\"none\" stroke-width=\"0.5\" stroke=\"black\" width=\"{width}\" height=\"{height}\" />" +
                   $"<text style=\"fill:black\" font-size=\"6\" transform=\"translate(-1,-1)\" >{text}</text>" +
                   "</g>";
        }
    }
}
