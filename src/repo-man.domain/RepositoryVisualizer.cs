using Microsoft.Extensions.Logging;
using repo_man.domain.Diagram;
using repo_man.domain.Git;

namespace repo_man.domain;

public class RepositoryVisualizer
{
    private readonly ILogger<RepositoryVisualizer> _logger;
    private readonly ITreeExtracter _extracter;
    private readonly IDiagramRenderer _renderer;

    public RepositoryVisualizer(ILogger<RepositoryVisualizer> logger, ITreeExtracter extracter, IDiagramRenderer renderer)
    {
        _logger = logger;
        _extracter = extracter;
        _renderer = renderer;
    }

    public async Task GenerateDiagram()
    {
        try
        {
            _logger.LogInformation("Extracting files from repository");
            var tree = _extracter.GetFileTree();
            _logger.LogInformation("Creating a diagram of the repository file tree");
            await _renderer.CreateDiagram(tree);
            _logger.LogInformation("Diagram creation complete!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating diagram. Exiting.");
        }
    }
}