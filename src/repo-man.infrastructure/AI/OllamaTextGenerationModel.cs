using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.TextGeneration;
using repo_man.domain.AI;
#pragma warning disable SKEXP0070

namespace repo_man.infrastructure.AI
{
    public class OllamaTextGenerationModel: ITextGenerationModel
    {
        private readonly IConfiguration _config;

        public OllamaTextGenerationModel(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> GenerateText(string systemPrompt)
        {
            var endpoint = _config["ollama:endpoint"]!;
            var model = _config["ollama:model"]!;

            var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10.0) };

            Kernel kernel = Kernel.CreateBuilder()
                .AddOllamaTextGeneration(
                    endpoint: new Uri(endpoint),
                    modelId: model, 
                    httpClient: client)
                .Build();

            var service = kernel.GetRequiredService<ITextGenerationService>();

            var result = await service.GetTextContentAsync(systemPrompt, 
                new OllamaPromptExecutionSettings{ Temperature = 0.8f } //default temperature = 0.8, higher for more creative, lower for more grounded
                );


            return result.ToString();
        }
    }
}
