﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using repo_man.domain.CodeQuality;
using repo_man.xunit._extensions;

namespace repo_man.xunit.domain.Diagram
{
    public class RepositoryVisualizerTest
    {
        private readonly AutoMocker _mocker = new();

        [Fact]
        public async Task ExtractsAndPushesTreeToDiagramRenderer()
        {
            //arrange
            var expected = new GitTree();
            _mocker.GetMock<ITreeExtracter>().Setup(te => te.GetFileTree()).Returns(expected);

            //act
            await WhenIGenerateADiagram();

            //assert
            _mocker.Verify<IDiagramRenderer>(r => r.CreateDiagram(expected), Times.Once);
            _mocker.Verify<IRiskIndexer>(r => r.DecorateTreeWithRiskIndex(expected), Times.Never);
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
        public async Task DecoratesTreeWithRiskIndexIfRiskDiagram()
        {
            _mocker.GetMock<IConfiguration>().Setup(c => c["type"]).Returns("risk");
            var tree = new GitTree();

            _mocker.GetMock<ITreeExtracter>().Setup(te => te.GetFileTree()).Returns(tree);

            await WhenIGenerateADiagram();

            _mocker.Verify<IRiskIndexer>(r => r.DecorateTreeWithRiskIndex(tree), Times.Once);
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
