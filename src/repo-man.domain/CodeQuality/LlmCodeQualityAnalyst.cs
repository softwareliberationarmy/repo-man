using System.Runtime.CompilerServices;
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
                @"Generate a report summarizing the code quality of the git repository. The report should include the following analyses:

1. **Tightly Coupled Files**: Identify files that are frequently modified together. These files may indicate a tight coupling issue.
2. **Large Files**: Identify files that are significantly larger than other files of the same type. These files may indicate areas of increased complexity.
3. **Frequent Bug Fixes**: Identify files where the commit messages indicate a high number of bug fixes. These files may be problematic.
4. **High Modification Frequency**: Identify files that are modified more frequently than others and which exhibit one of the previous characteristics (tight coupling, large size, frequent bug fixes) as potential red flags.

The report should use Markdown for formatting and follow this structure:

### Repository Health Report

#### Tightly Coupled Files
- **Example.java** and **Helper.java**: These files are frequently modified together, indicating a possible tight coupling issue.

#### Large Files
- **Example.java**: This file is significantly larger than other files of the same type, indicating potential complexity.

#### Frequent Bug Fixes
- **Example.java**: This file has a high number of bug fixes, which may indicate underlying issues.

#### High Modification Frequency
- **Example.java**: This file is modified more frequently than others and also has a high number of bug fixes, making it a potential red flag.

Please analyze the provided JSON data and generate the report accordingly.
");
            return builder.ToString();  

        }
    }
}
