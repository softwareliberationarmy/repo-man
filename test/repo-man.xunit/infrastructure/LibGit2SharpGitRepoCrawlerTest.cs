﻿using Microsoft.Extensions.Configuration;
using repo_man.infrastructure.Git;

namespace repo_man.xunit.infrastructure
{
    public class LibGit2SharpGitRepoCrawlerTest: TestBase
    {
        [Fact]
        [Obsolete]
        public void ThrowsExceptionWhenRepoPathNotConfigured()
        {
            _mocker.GetMock<IConfiguration>().Setup(c => c["repo"]).Returns((string?)null);

            var target = _mocker.CreateInstance<LibGit2SharpGitRepoCrawler>();

            Action getFiles = () => { target.GitThemFiles(); };
            getFiles.Should().Throw<InvalidOperationException>();
        }


    }
}
