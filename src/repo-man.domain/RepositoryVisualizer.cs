using Microsoft.Extensions.Logging;

namespace repo_man.domain;

public class RepositoryVisualizer
{
    private readonly ILogger<RepositoryVisualizer> _logger;

    public RepositoryVisualizer(ILogger<RepositoryVisualizer> logger)
    {
        _logger = logger;
    }

    public Task GenerateDiagram()
    {
        _logger.LogError("Repository path has not been specified. Exiting.");
        return Task.CompletedTask;
    }
}