using AutoFixture;
using repo_man.domain.Diagram.FileRadiusCalculator;

namespace repo_man.xunit.domain.Diagram.FileRadiusCalculator
{
    public class UnboundedFileRadiusCalculatorTest : TestBase
    {
        [Fact]
        public void Returns10ForMinFileSize()
        {
            var minFileSize = _fixture.Create<long>();

            var tree = new GitTree();
            tree.AddFile("Fred.cs", minFileSize, Array.Empty<Commit>());

            IFileRadiusCalculator target = new UnboundedFileRadiusCalculator();

            target.CalculateFileRadius(tree.Files.Single(), tree).Should().Be(10);
        }

        [Fact]
        public void Returns20IfFileTwiceAsLargeAsMin()
        {
            var minFileSize = Random.Shared.NextInt64(10L, short.MaxValue); //keep values conservative to prevent overflow

            var tree = new GitTree();
            tree.AddFile("Fred.cs", minFileSize, Array.Empty<Commit>());
            tree.AddFile("Program.cs", minFileSize * 2, Array.Empty<Commit>());

            IFileRadiusCalculator target = new UnboundedFileRadiusCalculator();

            target.CalculateFileRadius(tree.Files.Last(), tree).Should().Be(20);
        }
    }
}
