using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq.AutoMock;
using repo_man.infrastructure;

namespace repo_man.xunit.infrastructure
{
    public class LibGit2SharpGitRepoCrawlerTest
    {
        [Fact]
        public void ThrowsExceptionWhenRepoPathNotConfigured()
        {
            var mocker = new AutoMocker();
            mocker.GetMock<IConfiguration>().Setup(c => c["repo"]).Returns((string?)null);

            var target = mocker.CreateInstance<LibGit2SharpGitRepoCrawler>();

            Action getFiles = () => { target.GitThemFiles(); };
            getFiles.Should().Throw<InvalidOperationException>();
        }


    }
}
