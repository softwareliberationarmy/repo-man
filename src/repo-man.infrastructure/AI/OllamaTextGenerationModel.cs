using repo_man.domain.AI;

namespace repo_man.infrastructure.AI
{
    public class OllamaTextGenerationModel: ITextGenerationModel
    {
        public Task<string> GenerateText(string systemPrompt)
        {
            throw new NotImplementedException();
        }
    }
}
