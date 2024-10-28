using Microsoft.Extensions.Logging;
using Moq;
using repo_man.domain.CodeQuality;
using repo_man.xunit._extensions;

namespace repo_man.xunit.domain.CodeQuality
{
    public class RepositoryReviewerTest: TestBase
    {
        [Fact]
        public async Task GetsTheCommitHistoryAndPushesItToACodeQualityAnalystForReview()
        {
            var gitTree = new GitTree();
            _mocker.GetMock<ITreeExtracter>().Setup(t => t.GetFileTree()).Returns(gitTree);
            _mocker.GetMock<ICodeQualityAnalyst>().Setup(a => a.EvaluateCodeQuality(gitTree)).ReturnsAsync("LGTM");
            var target = _mocker.CreateInstance<RepositoryReviewer>();
            await target.ReviewCodeQuality();
            _mocker.GetMock<ICodeQualityAnalyst>().Verify(a => a.EvaluateCodeQuality(gitTree), Times.Once);
            _mocker.GetMock<ILogger<RepositoryReviewer>>()
                .VerifyInfoWasCalled("Starting code quality review", Times.Once());
            _mocker.GetMock<ILogger<RepositoryReviewer>>()
                .VerifyInfoWasCalled("Collecting git commit history", Times.Once());
            _mocker.GetMock<ILogger<RepositoryReviewer>>()
                .VerifyInfoWasCalled("Requesting review from code quality analyst", Times.Once());
            _mocker.GetMock<ILogger<RepositoryReviewer>>()
                .VerifyInfoWasCalled("Code quality review complete. Result: \r\nLGTM", Times.Once());
        }
    }
}
