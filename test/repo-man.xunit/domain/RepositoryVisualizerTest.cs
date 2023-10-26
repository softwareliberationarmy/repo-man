using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using repo_man.domain;
using repo_man.xunit._extensions;

namespace repo_man.xunit.domain
{
    public class RepositoryVisualizerTest
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        public async Task ExtractsAndPushesTreeToDiagramRenderer()
        {
            //arrange
            var expected = new Tree();
            _mocker.GetMock<ITreeExtracter>().Setup(te => te.GetFileTree()).Returns(expected);

            //act
            await WhenIGenerateADiagram();

            //assert
            _mocker.Verify<IDiagramRenderer>(r => r.RenderTree(expected), Times.Once);
        }

        [Fact]
        public async Task LogsHighLevelSteps()
        {
            //arrange

            //act
            await WhenIGenerateADiagram();

            //assert
            _mocker.GetMock<ILogger<RepositoryVisualizer>>()
                .VerifyInfoWasCalled("Extracting files from repository", Times.Once())
                .VerifyInfoWasCalled("Creating a diagram of the repository file tree", Times.Once())
                .VerifyInfoWasCalled("Diagram creation complete!", Times.Once());
        }

        [Fact]
        public async Task LogsExceptionAndExitsGracefully()
        {
            _mocker.GetMock<ITreeExtracter>().Setup(te => te.GetFileTree()).Throws<FileNotFoundException>();

            await WhenIGenerateADiagram();

            _mocker.GetMock<ILogger<RepositoryVisualizer>>()
                .VerifyErrorWasCalled((msg) => msg.Contains("Error generating diagram. Exiting."), Times.Once());
        }

        private async Task WhenIGenerateADiagram()
        {
            var target = _mocker.CreateInstance<RepositoryVisualizer>();
            await target.GenerateDiagram();
        }

    }
}
