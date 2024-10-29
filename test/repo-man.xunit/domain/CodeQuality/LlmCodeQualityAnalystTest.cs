using Moq;
using repo_man.domain.AI;
using repo_man.domain.CodeQuality;
using repo_man.domain.FileSystem;

namespace repo_man.xunit.domain.CodeQuality
{
    public class LlmCodeQualityAnalystTest : TestBase
    {
        [Fact]
        public async Task ItBuildsThePromptUsingTheJsonDataAndAPromptFileAndPassesItToTheModel()
        {
            var commitHistory = new List<(string, long, Commit[])>
            {
                ("master", 1, new Commit[] { new Commit("hash1") })
            };
            //massages the rag data using the builder
            _mocker.GetMock<IRagDataBuilder>().Setup(x => x.CreateCommitData(commitHistory)).Returns("The JSON data");

            //reads the prompt from a file
            _mocker.GetMock<IFileSystem>().Setup(fs => fs.ReadTextFromFileAsync(It.IsAny<string>()))
                .ReturnsAsync("Given this data: <<JSON>>, return me something");

            //calls the LLM
            _mocker.GetMock<ITextGenerationModel>().Setup(x => x.GenerateText("Given this data: The JSON data, return me something")).ReturnsAsync("LGTM");

            var target = _mocker.CreateInstance<LlmCodeQualityAnalyst>();

            var result = await target.EvaluateCodeQuality(commitHistory);
            result.Should().Be("LGTM");

            _mocker.VerifyAll();
        }
    }
}
