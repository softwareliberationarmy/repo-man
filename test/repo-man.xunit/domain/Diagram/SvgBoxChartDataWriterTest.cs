using AutoFixture;
using System.Drawing;
using Microsoft.Extensions.Logging;
using Moq;
using repo_man.xunit._extensions;
using repo_man.domain.Diagram.FileRadiusCalculator;
using repo_man.domain.Diagram.FileColorMapper;

namespace repo_man.xunit.domain.Diagram
{
    public class SvgBoxChartDataWriterTest: TestBase
    {
        [Theory]
        [InlineData("#0060ac", "Program.cs", ".cs")]
        [InlineData("green", "Program.cs", ".cs")]
        [InlineData("lightyellow", "CODEOWNERS", "CODEOWNERS")]
        [InlineData("aliceblue", "Main.cs", ".cs")]
        [InlineData("#012345", "Readme.md", ".md")]
        [InlineData("black", ".gitignore", ".gitignore")]
        public void Single_TopLevel_CS_File(string color, string fileName, string fileExtension)
        {
            GivenTheseColorMappings(new Tuple<string, string>(fileExtension, color));

            var tree = GivenThisFileTree(
                new Tuple<string, long>(fileName, _fixture.Create<long>()));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle(color, new Point(20,20), 10, fileName));
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

            result.Data.Should().Be(AFilledCircle("pink", new Point(20,20), 10, "Program.cs") +
                                    AFilledCircle("blue", new Point(45, 20), 10, "Readme.md"));
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
                AFilledCircle("#001122", new Point(30,30), 20, "Program.cs") +
                AFilledCircle("#001122", new Point(65,20), 10, "App.cs"));
        }

        [Fact]
        public void TopLevel_Files_Are_Ordered_By_Size_Descending()
        {
            GivenTheseColorMappings(new Tuple<string, string>(".cs", "#001122"));
            var tree = GivenThisFileTree(
                new Tuple<string, long>("App.cs", 50),
                new Tuple<string, long>("Program.cs", 100));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle("#001122", new Point(30,30), 20, "Program.cs") +
                                    AFilledCircle("#001122", new Point(65,20), 10, "App.cs"));
        }

        [Fact]
        public void Single_File_In_A_Folder()
        {
            GivenTheseColorMappings(new Tuple<string, string>(".cs", "fuschia"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/Program.cs", _fixture.Create<long>()));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle("fuschia", new Point(25,25), 10, "Program.cs") +
                                    ARectangle(new Point(10, 10), 30, 30, "src"));
        }

        [Fact]
        public void Two_Files_In_A_Folder_Same_Size()
        {
            GivenTheseColorMappings(new Tuple<string, string>(".cs", "fuschia"), new Tuple<string, string>(".xaml", "#001122"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/Program.cs", 100L),
                new Tuple<string, long>("src/App.xaml", 100L));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle("fuschia", new Point(25, 25), 10, "Program.cs") +
                                    AFilledCircle("#001122", new Point(50, 25), 10, "App.xaml") +
                                    ARectangle(new Point(10, 10), 55, 30, "src"));
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

            result.Data.Should().Be(AFilledCircle("#001122", new Point(35,35), 20, "App.xaml") +
                                    AFilledCircle("fuschia", new Point(70,25), 10, "Program.cs") +
                                    ARectangle(new Point(10, 10), 75, 50, "src"));
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

            result.Data.Should().Be(AFilledCircle("blue", new Point(45,45), 30, "Bootstrapper.cs") +
                                    AFilledCircle("blue", new Point(90,25), 10, "Program.cs") +
                                    ARectangle(new Point(10,10), 95, 70, "src") +
                                    AFilledCircle("pink", new Point(55,135), 40, "GettingStarted.md") +
                                    AFilledCircle("pink", new Point(120,115), 20, "About.md") +
                                    ARectangle(new Point(10,90), 135, 90, "docs"));
        }

        [Fact]
        public void TopLevelFiles_Two_Folders_Different_Sizes_Stacked()
        {
            GivenTheseColorMappings(
                new Tuple<string, string>(".cs", "blue"),
                new Tuple<string, string>(".gitignore", "white"),
                new Tuple<string, string>(".md", "pink"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/Program.cs", 100L),
                new Tuple<string, long>("src/Bootstrapper.cs", 300L),
                new Tuple<string, long>("docs/About.md", 200L),
                new Tuple<string, long>("README.md", 50L),
                new Tuple<string, long>(".gitignore", 500L),
                new Tuple<string, long>("docs/GettingStarted.md", 400L));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle("white", new Point(110, 110), 100, ".gitignore") + 
                                    AFilledCircle("pink", new Point(225, 20), 10, "README.md") +
                                    AFilledCircle("blue", new Point(75, 285), 60, "Bootstrapper.cs") +
                                    AFilledCircle("blue", new Point(160, 245), 20, "Program.cs") +
                                    ARectangle(new Point(10, 220), 175, 130, "src") +
                                    AFilledCircle("pink", new Point(95, 445), 80, "GettingStarted.md") +
                                    AFilledCircle("pink", new Point(220, 405), 40, "About.md") +
                                    ARectangle(new Point(10, 360), 255, 170, "docs"));
        }

        [Fact]
        public void TwoNestedFolders_InsideOneParentFolder()
        {
            GivenTheseColorMappings(
                new Tuple<string, string>(".cs", "blue"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/console/Program.cs", 100L),
                new Tuple<string, long>("src/console/Bootstrapper.cs", 300L),
                new Tuple<string, long>("src/domain/Context.cs", 200L),
                new Tuple<string, long>("src/domain/Model.cs", 400L));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle("blue", new Point(55, 55), 30, "Bootstrapper.cs") +
                                    AFilledCircle("blue", new Point(100, 35), 10, "Program.cs") +
                                    ARectangle(new Point(20, 20), 95, 70, "console") +
                                    AFilledCircle("blue", new Point(65, 145), 40, "Model.cs") +
                                    AFilledCircle("blue", new Point(130, 125), 20, "Context.cs") +
                                    ARectangle(new Point(20, 100), 135, 90, "domain") +
                                    ARectangle(new Point(10, 10), 155, 190, "src"));
        }

        [Fact]
        public void SingleFile_DeeplyNested()
        {
            GivenTheseColorMappings(
                new Tuple<string, string>(".cs", "blue"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/domain/console/Program.cs", 60000L));

            var result = WhenICreateChartData(tree);

            result.Data.Should().Be(AFilledCircle("blue", new Point(45, 45), 10, "Program.cs") +
                                    ARectangle(new Point(30, 30), 30, 30, "console") +
                                    ARectangle(new Point(20, 20), 50, 50, "domain") +
                                    ARectangle(new Point(10, 10), 70, 70, "src"));
        }

        [Fact]
        public void Single_File_Returns_Max_Height_And_Width_With_Chart_Data()
        {
            GivenTheseColorMappings(
                new Tuple<string, string>(".cs", "blue"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("Program.cs", 60000L));

            var result = WhenICreateChartData(tree);

            result.Size.Should().BeEquivalentTo(new Point(35,40));  //circle x ends at 30 + 5 for interfile margin, circle y ends at 30 + 10 for top file bottom margin
        }

        [Fact]
        public void TopFiles_Returns_Max_Height_And_Width_With_Chart_Data()
        {
            GivenTheseColorMappings(
                new Tuple<string, string>(".cs", "blue"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("Program.cs", 100L),
                new Tuple<string, long>("Initializer.cs", 100L),
                new Tuple<string, long>("Committer.cs", 100L),
                new Tuple<string, long>("src/otherfile.cs", 100L)
                );

            var result = WhenICreateChartData(tree);

            result.Size.Should().BeEquivalentTo(new Point(85, 70));  
            //x = 10 margin + 20 radius + 5 margin + 20 radius + 5 margin + 20 radius + 5 margin (toplevel > boxes)
            //y = 10 margin + 20 radius + 10 margin + 5 padding + 20 radius + 5 padding
        }

        [Fact]
        public void Logs_HighLevelFlow_Of_DataWriter()
        {
            GivenTheseColorMappings(
                new Tuple<string, string>(".cs", "blue"),
                new Tuple<string, string>(".gitignore", "white"),
                new Tuple<string, string>(".md", "pink"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/Program.cs", 100L),
                new Tuple<string, long>("README.md", 50L),
                new Tuple<string, long>("docs/GettingStarted.md", 400L));

            WhenICreateChartData(tree);

            _mocker.GetMock<ILogger<SvgBoxChartDataWriter>>().VerifyInfoWasCalled("Writing top-level files", Times.Once());
            _mocker.GetMock<ILogger<SvgBoxChartDataWriter>>().VerifyInfoWasCalled("Writing foldered files", Times.Once());
        }

        [Fact]
        public void Logs_Individual_File_Names()
        {
            GivenTheseColorMappings(
                new Tuple<string, string>(".cs", "blue"),
                new Tuple<string, string>(".md", "pink"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/Program.cs", 100L),
                new Tuple<string, long>("README.md", 50L),
                new Tuple<string, long>("docs/GettingStarted.md", 400L));

            WhenICreateChartData(tree);

            _mocker.GetMock<ILogger<SvgBoxChartDataWriter>>().VerifyInfoWasCalled("Program.cs", Times.Once());
            _mocker.GetMock<ILogger<SvgBoxChartDataWriter>>().VerifyInfoWasCalled("README.md", Times.Once());
            _mocker.GetMock<ILogger<SvgBoxChartDataWriter>>().VerifyInfoWasCalled("GettingStarted.md", Times.Once());
        }

        [Fact]
        public void Logs_Individual_Folders()
        {
            GivenTheseColorMappings(
                new Tuple<string, string>(".cs", "blue"),
                new Tuple<string, string>(".md", "pink"));

            var tree = GivenThisFileTree(
                new Tuple<string, long>("src/console/Program.cs", 100L),
                new Tuple<string, long>("README.md", 50L),
                new Tuple<string, long>("docs/GettingStarted.md", 400L));

            WhenICreateChartData(tree);

            _mocker.GetMock<ILogger<SvgBoxChartDataWriter>>().VerifyInfoWasCalled("src/", Times.Once());
            _mocker.GetMock<ILogger<SvgBoxChartDataWriter>>().VerifyInfoWasCalled("console/", Times.Once());
            _mocker.GetMock<ILogger<SvgBoxChartDataWriter>>().VerifyInfoWasCalled("docs/", Times.Once());
        }

        #region helper methods
        private ChartData WhenICreateChartData(GitTree tree)
        {
            _mocker.Use<IFileRadiusCalculator>(new UnboundedFileRadiusCalculator());
            var target = _mocker.CreateInstance<SvgBoxChartDataWriter>();
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

        private static string AFilledCircle(string color, Point point, long expectedRadius,
            string fileName)
        {
            return $"<g style=\"fill:{color}\" transform=\"translate({point.X},{point.Y})\">" +
                   $"<circle r=\"{expectedRadius}\" />" +
                   $"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\" >{fileName}</text>" +
                   "</g>";
        }

        private string ARectangle(Point point, long width, long height, string text)
        {
            return $"<g transform=\"translate({point.X},{point.Y})\">" +
                   $"<rect fill=\"none\" stroke-width=\"0.5\" stroke=\"black\" width=\"{width}\" height=\"{height}\" />" +
                   $"<text style=\"fill:black\" font-size=\"6\" transform=\"translate(-1,-1)\" >{text}</text>" +
                   "</g>";
        }
        #endregion

    }
}
