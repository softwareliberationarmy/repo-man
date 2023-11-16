using FluentAssertions;
using repo_man.domain.Diagram;
using repo_man.domain.Git;
using repo_man.xunit._helpers;

namespace repo_man.xunit.domain.Diagram
{

    public class BoundedFileRadiusCalculatorTest: TestBase
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

        //TODO: check for divide by zero errors
        //TODO: allow max radius to be configurable

    }

    public class BoundedFileRadiusCalculator : IFileRadiusCalculator
    {
        public int CalculateFileRadius(GitFile file, GitTree gitTree)
        {
            const int minRadius = 10;
            const int maxRadius = 100;

            var minFileSize = gitTree.GetMinFileSize();
            var maxFileSize = gitTree.GetMaxFileSize();

            if (file.FileSize == minFileSize)
            {
                return minRadius;
            }
            
            var percent = (double)(file.FileSize - minFileSize) / (maxFileSize - minFileSize);
            var increment = (int)Math.Round(percent * (maxRadius - minRadius));
            var fileRadius = minRadius + increment;

            return fileRadius;
        }
    }
}
