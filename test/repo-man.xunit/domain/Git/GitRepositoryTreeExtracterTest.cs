using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using repo_man.domain.Git;
using repo_man.xunit._extensions;
using repo_man.xunit._helpers;

namespace repo_man.xunit.domain.Git
{
    public class GitRepositoryTreeExtracterTest : TestBase
    {
        [Fact]
        public void ReturnsAnEmptyTreeIfNoFiles()
        {
            _mocker.GetMock<IGitRepoCrawler>().Setup(x => x.GitThemFiles(false))
                .Returns(Enumerable.Empty<(string, long, Commit[])>());

            var target = _mocker.CreateInstance<GitRepositoryTreeExtracter>();
            var result = target.GetFileTree();

            result.Should().NotBeNull();
            result.Files.Should().BeEmpty();
            result.Folders.Should().BeEmpty();
        }

        [Fact]
        public void AllGitFilesReturnedAreStoredInTree()
        {
            var expectedLengths = _fixture.CreateMany<long>(2).ToArray();
            _mocker.GetMock<IGitRepoCrawler>().Setup(x => x.GitThemFiles(false))
                .Returns(new[]
                {
                    ("readme.md", expectedLengths[0], Array.Empty<Commit>()),
                    ("kung/fu.json", expectedLengths[1], Array.Empty<Commit>())
                }.AsEnumerable());

            var target = _mocker.CreateInstance<GitRepositoryTreeExtracter>();
            var result = target.GetFileTree();

            result.Should().NotBeNull();
            result.Files.Single().Should().BeEquivalentTo(new GitFile("readme.md", expectedLengths[0], Array.Empty<Commit>()));
            result.Folders.Single().Name.Should().Be("kung");
            result.Folders.Single().Files.Single().Should()
                .BeEquivalentTo(new GitFile("fu.json", expectedLengths[1], Array.Empty<Commit>()));
        }

        [Fact]
        public void LogsInfoMessagesForEachFile()
        {
            _mocker.GetMock<IGitRepoCrawler>().Setup(x => x.GitThemFiles(false))
                .Returns(new[]
                {
                    ("readme.md", 200L, Array.Empty<Commit>()),
                    ("kung/fu.json", 1000L, Array.Empty<Commit>())
                }.AsEnumerable());

            var target = _mocker.CreateInstance<GitRepositoryTreeExtracter>();
            target.GetFileTree();

            _mocker.GetMock<ILogger<GitRepositoryTreeExtracter>>().VerifyInfoWasCalled(
                "Collecting files from repository", Times.Once());
            _mocker.GetMock<ILogger<GitRepositoryTreeExtracter>>().VerifyInfoWasCalled(
                "Adding file readme.md - 200 bytes - 0 commits", Times.Once());
            _mocker.GetMock<ILogger<GitRepositoryTreeExtracter>>().VerifyInfoWasCalled(
                "Adding file kung/fu.json - 1000 bytes - 0 commits", Times.Once());
        }
    }
}
