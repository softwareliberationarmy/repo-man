using FluentAssertions;
using repo_man.domain;
using repo_man.domain.Git;

namespace repo_man.xunit.domain
{
    public class TreeTest
    {
        [Fact]
        public void TopLevelFileSavedToTree()
        {
            var target = new Tree();
            var expectedHash = "abcdefghiasd";
            target.AddFile("readme.md", long.Parse("123456"),  new[] { new Commit(expectedHash, DateTimeOffset.Now) });

            target.TopLevelFiles.Single().Commits.Single().Hash.Should().Be(expectedHash);
            target.TopLevelFiles.Single().Commits.Single().CommitDate.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(5.0));
        }


    }
}
