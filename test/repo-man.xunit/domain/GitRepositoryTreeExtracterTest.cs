using FluentAssertions;
using Moq.AutoMock;
using repo_man.domain.Git;

namespace repo_man.xunit.domain
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
    }
}
