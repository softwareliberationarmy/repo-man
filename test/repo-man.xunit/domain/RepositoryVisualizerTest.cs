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
        public async Task ExtractsAndPushesTreeToDiagramRenderer()
        {
            var mocker = new AutoMocker();
            var tree = new Tree();
            
            mocker.GetMock<ITreeExtracter>().Setup(te => te.GetFileTree()).Returns(tree);

            var target = mocker.CreateInstance<RepositoryVisualizer>();

            await target.GenerateDiagram();

            mocker.Verify<IDiagramRenderer>(r => r.RenderTree(tree), Times.Once);
        }

        [Fact]
        public async Task LogsHighLevelSteps()
        {
            var mocker = new AutoMocker();

            var target = mocker.CreateInstance<RepositoryVisualizer>();

            await target.GenerateDiagram();

            mocker.GetMock<ILogger<RepositoryVisualizer>>().VerifyInfoWasCalled("Extracting files from repository", Times.Once());
            mocker.GetMock<ILogger<RepositoryVisualizer>>().VerifyInfoWasCalled("Creating a diagram of the repository file tree", Times.Once());
            mocker.GetMock<ILogger<RepositoryVisualizer>>().VerifyInfoWasCalled("Diagram creation complete!", Times.Once());
        }
    }
}
