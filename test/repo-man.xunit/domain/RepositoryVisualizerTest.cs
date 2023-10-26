using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using repo_man.domain;
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
}
