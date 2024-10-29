using repo_man.domain.CodeQuality;
using repo_man.domain.Git;

namespace repo_man.infrastructure.AI
{
    public class OllamaCodeQualityAnalyst: ICodeQualityAnalyst
    {
        public Task<string> EvaluateCodeQuality(IEnumerable<(string, long, Commit[])> commitHistory)
        {
            //TODO: implement Ollama code quality analyst
            return Task.FromResult("LGTM");
        }
    }
}
