using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq.AutoMock;
using repo_man.infrastructure;
using repo_man.xunit._helpers;

namespace repo_man.xunit.infrastructure
{
    public class LibGit2SharpGitRepoCrawlerTest: TestBase
    {
        [Fact]
        public void ThrowsExceptionWhenRepoPathNotConfigured()
        {
            _mocker.GetMock<IConfiguration>().Setup(c => c["repo"]).Returns((string?)null);

            var target = _mocker.CreateInstance<LibGit2SharpGitRepoCrawler>();

            Action getFiles = () => { target.GitThemFiles(); };
            getFiles.Should().Throw<InvalidOperationException>();
        }


    }
}
