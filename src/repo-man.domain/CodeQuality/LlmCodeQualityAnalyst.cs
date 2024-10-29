using System.Text;
using repo_man.domain.AI;
using repo_man.domain.FileSystem;
using repo_man.domain.Git;

namespace repo_man.domain.CodeQuality
{
    public class LlmCodeQualityAnalyst: ICodeQualityAnalyst
    {
        private readonly IRagDataBuilder _ragDataBuilder;
        private readonly ITextGenerationModel _textGenerationModel;
        private readonly IFileSystem _fileSystem;

        public LlmCodeQualityAnalyst(IRagDataBuilder ragDataBuilder, ITextGenerationModel textGenerationModel, IFileSystem fileSystem)
        {
            _ragDataBuilder = ragDataBuilder;
            _textGenerationModel = textGenerationModel;
            _fileSystem = fileSystem;
        }

        public async Task<string> EvaluateCodeQuality(IEnumerable<(string, long, Commit[])> commitHistory)
        {
            var commitData = _ragDataBuilder.CreateCommitData(commitHistory);

            var prompt = await _fileSystem.ReadTextFromFileAsync("AI/prompts/analyze-code-quality.prompt");
            prompt = prompt.Replace("<<JSON>>", commitData);

            return await _textGenerationModel.GenerateText(prompt);
        }

    }
}
