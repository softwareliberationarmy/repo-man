using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using repo_man.xunit._extensions;

namespace repo_man.xunit.domain
{
    public class RepositoryVisualizerTest
    {
        [Fact]
        public async Task ExitsEarlyIfRepoPathCannotBeFound()
        {
            var mocker = new AutoMocker();
            var target = mocker.CreateInstance<RepositoryVisualizer>();

            await target.GenerateDiagram();

            mocker.GetMock<ILogger<RepositoryVisualizer>>()
                .VerifyErrorWasCalled("Repository path has not been specified. Exiting.", Times.Once());
        }
}

    public class RepositoryVisualizer
    {
        private readonly ILogger<RepositoryVisualizer> _logger;

        public RepositoryVisualizer(ILogger<RepositoryVisualizer> logger)
        {
            _logger = logger;
        }

        public Task GenerateDiagram()
        {
            _logger.LogError("Repository path has not been specified. Exiting.");
            return Task.CompletedTask;
        }
    }
}
