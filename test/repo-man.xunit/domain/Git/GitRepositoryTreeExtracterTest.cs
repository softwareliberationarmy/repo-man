using AutoFixture;
using FluentAssertions;
using Moq.AutoMock;
using repo_man.domain.Git;

namespace repo_man.xunit.domain.Git
{
    public class GitRepositoryTreeExtracterTest
    {
        [Fact]
        public void ReturnsAnEmptyTreeIfNoFiles()
        {
            var mocker = new AutoMocker();
            mocker.GetMock<IGitRepoCrawler>().Setup(x => x.GetFilesWithCommits())
                .Returns(Enumerable.Empty<(string, long, Commit[])>());

            var target = mocker.CreateInstance<GitRepositoryTreeExtracter>();
            var result = target.GetFileTree();

            result.Should().NotBeNull();
            result.Files.Should().BeEmpty();
            result.Folders.Should().BeEmpty();
        }

        [Fact]
        public void AllGitFilesReturnedAreStoredInTree()
        {
            var fixture = new Fixture();
            var expectedLengths = fixture.CreateMany<long>(2).ToArray();
            var mocker = new AutoMocker();
            mocker.GetMock<IGitRepoCrawler>().Setup(x => x.GetFilesWithCommits())
                .Returns(new[]
                {
                    ("readme.md", expectedLengths[0], new Commit[0]),
                    ("kung/fu.json", expectedLengths[1], new Commit[0])
                }.AsEnumerable());

            var target = mocker.CreateInstance<GitRepositoryTreeExtracter>();
            var result = target.GetFileTree();

            result.Should().NotBeNull();
            result.Files.Single().Should().BeEquivalentTo(new GitFile("readme.md", expectedLengths[0], new Commit[0]));
            result.Folders.Single().Name.Should().Be("kung");
            result.Folders.Single().Files.Single().Should()
                .BeEquivalentTo(new GitFile("fu.json", expectedLengths[1], new Commit[0]));
        }
    }
}
