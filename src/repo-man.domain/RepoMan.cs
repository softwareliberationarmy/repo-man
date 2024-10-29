using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using repo_man.domain.CodeQuality;
using repo_man.domain.Diagram;

namespace repo_man.domain;

public class RepoMan
{
    private readonly IConfiguration _config;
    private readonly ILogger<RepoMan> _logger;
    private readonly RepositoryVisualizer _repoVisualizer;
    private readonly RepositoryReviewer _repoReviewer;

    public RepoMan(IConfiguration config, ILogger<RepoMan> logger, RepositoryVisualizer repoVisualizer, RepositoryReviewer repoReviewer)
    {
        _config = config;
        _logger = logger;
        _repoVisualizer = repoVisualizer;
        _repoReviewer = repoReviewer;
    }

    public async Task Run()
    {
        //_logger.LogInformation($"Endpoint: {_config["ollama:endpoint"]}");
        //_logger.LogInformation($"Endpoint: {_config["ollama:model"]}");

        var action = (_config["action"] ?? "").ToLower();

        if (action == "diagram")
        {
            await _repoVisualizer.GenerateDiagram();
        }
        else if (action == "review")
        {
            await _repoReviewer.ReviewCodeQuality();
        }
        else
        {
            _logger.LogError("Unknown action requested. No support for action '{action}'", action);
        }
    }
}