using Microsoft.Extensions.Logging;

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
        _logger.LogInformation("Extracting files from repository");
        var tree = _extracter.GetFileTree();
        _logger.LogInformation("Creating a diagram of the repository file tree");
        await _renderer.RenderTree(tree);
        _logger.LogInformation("Diagram creation complete!");
    }
}