using AutoFixture;
using FluentAssertions;
using repo_man.domain.Diagram;
using repo_man.domain.Git;
using repo_man.xunit._helpers;

namespace repo_man.xunit.domain.Diagram
{
    public class UnboundedFileRadiusCalculatorTest: TestBase
    {
        [Fact]
        public void Returns10ForMinFileSize()
        {
            var minFileSize = _fixture.Create<long>();

            var tree = new GitTree();
            tree.AddFile("Fred.cs", minFileSize, Array.Empty<Commit>());

            var target = new UnboundedFileRadiusCalculator(tree);

            target.CalculateFileRadius(tree.Files.Single()).Should().Be(10);
        }

        [Fact]
        public void Returns20IfFileTwiceAsLargeAsMin()
        {
            var minFileSize = Random.Shared.NextInt64(10L, Int16.MaxValue); //keep values conservative to prevent overflow

            var tree = new GitTree();
            tree.AddFile("Fred.cs", minFileSize, Array.Empty<Commit>());
            tree.AddFile("Program.cs", minFileSize * 2, Array.Empty<Commit>());

            var target = new UnboundedFileRadiusCalculator(tree);

            target.CalculateFileRadius(tree.Files.Last()).Should().Be(20);
        }
    }
}
