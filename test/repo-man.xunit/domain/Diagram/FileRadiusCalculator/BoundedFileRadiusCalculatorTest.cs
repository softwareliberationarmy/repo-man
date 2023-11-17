using Microsoft.Extensions.Configuration;
using repo_man.domain.Diagram.FileRadiusCalculator;

namespace repo_man.xunit.domain.Diagram.FileRadiusCalculator
{

    public class BoundedFileRadiusCalculatorTest : TestBase
    {
        [Fact]
        public void Sets_Smallest_File_Radius_To_10()
        {
            var tree = GivenThisFileTree(
                new Tuple<string, long>("program.cs", 100L),
                new Tuple<string, long>("readme.md", 10L)
                );

            var radius = WhenICalculateTheFileRadius(tree.Files.Last(), tree);
            radius.Should().Be(10);
        }

        [Fact]
        public void Sets_Largest_File_Radius_To_100()
        {
            var tree = GivenThisFileTree(
                new Tuple<string, long>("program.cs", 100L),
                new Tuple<string, long>("readme.md", 10L)
            );

            var radius = WhenICalculateTheFileRadius(tree.Files.First(), tree);
            radius.Should().Be(100);
        }

        [Fact]
        public void Sets_Midsized_File_To_55()
        {
            var tree = GivenThisFileTree(
                new Tuple<string, long>("readme2.md", 20L), //file size is exactly the midpoint between min and max
                new Tuple<string, long>("readme.md", 10L),
                new Tuple<string, long>("readme3.md", 30L)
            );

            var radius = WhenICalculateTheFileRadius(tree.Files.First(), tree);
            radius.Should().Be(55); //(max - min)/2 + min
        }

        [Fact]
        public void Proportionately_Sizes_Files_Across_A_Spectrum()
        {
            var tree = GivenThisFileTree(
                new Tuple<string, long>("readme2.md", 20L), //file is 1/3 of way between min and max
                new Tuple<string, long>("readme.md", 10L),  // file is min size
                new Tuple<string, long>("readme4.md", 40L), //file is max size
                new Tuple<string, long>("readme3.md", 30L)  //file is 2/3 of way between min and max
            );

            WhenICalculateTheFileRadius(tree.Files.First(), tree).Should().Be(40); // 1/3 of the way along the spectrum (total spectrum = 90, so 30 + start (10))
            WhenICalculateTheFileRadius(tree.Files.Skip(1).First(), tree).Should().Be(10);  //the min radius
            WhenICalculateTheFileRadius(tree.Files.Skip(2).First(), tree).Should().Be(100);  //the max radius
            WhenICalculateTheFileRadius(tree.Files.Last(), tree).Should().Be(70);  //2/3 of the way along the spectrum (total spectrum = 90, so 60 + start (10))
        }

        [Fact]
        public void When_Just_One_File_Radius_Is_10()
        {
            var tree = GivenThisFileTree(new Tuple<string, long>("readme.md", 10L));

            var result = WhenICalculateTheFileRadius(tree.Files.First(), tree);

            result.Should().Be(10);
        }

        [Fact]
        public void When_All_Files_Same_Size_Radius_Is_10()
        {
            var tree = GivenThisFileTree(
                new Tuple<string, long>("readme.md", 10L),
                new Tuple<string, long>("gettingstarted.md", 10L)
                );

            WhenICalculateTheFileRadius(tree.Files.First(), tree).Should().Be(10);
            WhenICalculateTheFileRadius(tree.Files.Last(), tree).Should().Be(10);
        }

        [Theory]
        [InlineData("500", 500)]
        [InlineData("10", 10)]
        public void Largest_File_Radius_IsConfigurable(string configInput, int expectedRadius)
        {
            _mocker.GetMock<IConfiguration>().Setup(c => c["maxRadius"]).Returns(configInput);

            var tree = GivenThisFileTree(
                new Tuple<string, long>("program.cs", 100L),
                new Tuple<string, long>("readme.md", 10L)
            );

            var radius = WhenICalculateTheFileRadius(tree.Files.First(), tree);
            radius.Should().Be(expectedRadius);
        }

        [Theory]
        [InlineData(null)]    //empty string
        [InlineData("")]    //empty string
        [InlineData("abc")] //not numeric
        [InlineData("500.0001")]    //decimal value
        [InlineData("-1")]  //negative value
        [InlineData("9")]  //less than minRadius
        public void Ignores_Invalid_MaxRadius_Config_Values(string configValue)
        {
            _mocker.GetMock<IConfiguration>().Setup(c => c["maxRadius"]).Returns(configValue);

            var tree = GivenThisFileTree(
                new Tuple<string, long>("program.cs", 100L),
                new Tuple<string, long>("readme.md", 10L)
            );

            var radius = WhenICalculateTheFileRadius(tree.Files.First(), tree);
            radius.Should().Be(100);
        }

        private int WhenICalculateTheFileRadius(GitFile gitFile, GitTree tree)
        {
            IFileRadiusCalculator target = _mocker.CreateInstance<BoundedFileRadiusCalculator>();
            var radius = target.CalculateFileRadius(gitFile, tree);
            return radius;
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

    }
}
