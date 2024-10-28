using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using repo_man.domain;
using repo_man.domain.CodeQuality;
using repo_man.xunit._extensions;

namespace repo_man.xunit.domain
{
    public class RepoManTest: TestBase
    {
        [Theory]
        [InlineData("diagram")]
        [InlineData("Diagram")]
        [InlineData("DIAGRAM")]
        public async Task LaunchesRepoVisualizerIfConfigActionIsDiagram(string diagramAction)
        {
            GivenAnActionOf(diagramAction);
            await WhenIRunTheRepoMan();
            _mocker.GetMock<RepositoryVisualizer>().Verify(v => v.GenerateDiagram(), Times.Once);
        }

        [Theory]
        [InlineData("review")]
        [InlineData("Review")]
        [InlineData("REVIEW")]
        public async Task LaunchesRepoReviewerIfConfigActionIsReview(string reviewAction)
        {
            GivenAnActionOf(reviewAction);
            await WhenIRunTheRepoMan();
            _mocker.GetMock<RepositoryReviewer>().Verify(v => v.ReviewCodeQuality(), Times.Once);
        }

        [Fact]
        public async Task LaunchesNeitherIfConfigActionIsUnexpected()
        {
            GivenAnActionOf(Guid.NewGuid().ToString());
            await WhenIRunTheRepoMan();
            _mocker.GetMock<RepositoryVisualizer>().Verify(v => v.GenerateDiagram(), Times.Never);
            _mocker.GetMock<RepositoryReviewer>().Verify(v => v.ReviewCodeQuality(), Times.Never);
        }

        [Fact]
        public async Task LogsErrorIfConfigActionIsUnexpected()
        {
            GivenAnActionOf("migrateToSvn");
            await WhenIRunTheRepoMan();
            _mocker.GetMock<ILogger<RepoMan>>().VerifyErrorWasCalled(m => m.Contains("Unknown action requested"), Times.Once());
        }

        #region helpers
        private async Task WhenIRunTheRepoMan()
        {
            var repoMan = _mocker.CreateInstance<RepoMan>();
            await repoMan.Run();
        }

        private void GivenAnActionOf(string action)
        {
            _mocker.GetMock<IConfiguration>().Setup(c => c["action"]).Returns(action);
        }
        #endregion
    }
}
