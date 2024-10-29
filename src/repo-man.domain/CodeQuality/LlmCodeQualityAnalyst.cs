using System.Text;
using repo_man.domain.AI;
using repo_man.domain.Git;

namespace repo_man.domain.CodeQuality
{
    public class LlmCodeQualityAnalyst: ICodeQualityAnalyst
    {
        private readonly IRagDataBuilder _ragDataBuilder;
        private readonly ITextGenerationModel _textGenerationModel;

        public LlmCodeQualityAnalyst(IRagDataBuilder ragDataBuilder, ITextGenerationModel textGenerationModel)
        {
            _ragDataBuilder = ragDataBuilder;
            _textGenerationModel = textGenerationModel;
        }

        public Task<string> EvaluateCodeQuality(IEnumerable<(string, long, Commit[])> commitHistory)
        {
            var commitData = _ragDataBuilder.CreateCommitData(commitHistory);

            var prompt = BuildPromptWith(commitData);

            return _textGenerationModel.GenerateText(prompt);
        }

        private string BuildPromptWith(string commitData)
        {
            var builder = new StringBuilder();
            builder.AppendLine(
                "You are a software engineering code quality analyst. Your job is to review codebases and report on their code quality.");
            builder.AppendLine("Given the following data extracted from the git commit history of a git repo:");
            builder.AppendLine("```");
            builder.AppendLine(commitData);
            builder.AppendLine("```");
            builder.AppendLine(
                @"Generate a report summarizing the code quality of the git repository and identifying any code quality risks. The report should use Markdown for formatting.

Code quality risks may include:

1. **Tightly Coupled Files**: Look for files that have the same commit hashes in their commit history. These files are frequently modified together, indicating a possible tight coupling issue.
2. **Large Files**: Large files sometimes indicate areas of complexity. Identify files that are significantly larger than other files of the same type.
3. **Frequent Bug Fixes**: Look for files that have several commits with a commit message indicating a bug fix. These files may be encountering recurring bugs due to code quality issues.  

Additionally, files which are frequently modified and fit one of the above categories may be high risk. 

Please analyze the provided JSON data and generate the report accordingly.
");
            return builder.ToString();  

        }
    }
}
