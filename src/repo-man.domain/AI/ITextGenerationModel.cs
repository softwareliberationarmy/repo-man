namespace repo_man.domain.AI;

public interface ITextGenerationModel
{
    Task<string> GenerateText(string systemPrompt);
}