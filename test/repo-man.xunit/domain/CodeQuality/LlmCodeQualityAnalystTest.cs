using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using repo_man.domain.AI;
using repo_man.domain.CodeQuality;

namespace repo_man.xunit.domain.CodeQuality
{
    public class LlmCodeQualityAnalystTest : TestBase
    {
        [Fact]
        public async Task ItBuildsThePromptUsingTheJsonDataAndPassesItToTheModel()
        {
            var commitHistory = new List<(string, long, Commit[])>
            {
                ("master", 1, new Commit[] { new Commit("hash1") })
            };
            _mocker.GetMock<IRagDataBuilder>().Setup(x => x.CreateCommitData(commitHistory)).Returns("The JSON data");
            _mocker.GetMock<ITextGenerationModel>().Setup(x => x.GenerateText(It.Is<string>(prompt => prompt.Contains("The JSON data")))).ReturnsAsync("LGTM");

            var target = _mocker.CreateInstance<LlmCodeQualityAnalyst>();

            var result = await target.EvaluateCodeQuality(commitHistory);
            result.Should().Be("LGTM");

            _mocker.VerifyAll();
        }
    }
}
