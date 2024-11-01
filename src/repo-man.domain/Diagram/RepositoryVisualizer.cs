using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using repo_man.domain.CodeQuality;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class RepositoryVisualizer
{
    private readonly ILogger<RepositoryVisualizer> _logger;
    private readonly ITreeExtracter _extracter;
    private readonly IDiagramRenderer _renderer;
    private readonly IRiskIndexer _riskIndexer;
    private readonly IConfiguration _configuration;

    public RepositoryVisualizer(ILogger<RepositoryVisualizer> logger, ITreeExtracter extracter, IDiagramRenderer renderer, IRiskIndexer riskIndexer, IConfiguration configuration)
    {
        _logger = logger;
        _extracter = extracter;
        _renderer = renderer;
        _riskIndexer = riskIndexer;
        _configuration = configuration;
    }

    public virtual async Task GenerateDiagram()
    {
        try
        {
            _logger.LogInformation("Extracting files from repository");
            var tree = _extracter.GetFileTree();

            if(_configuration["type"] == "risk")
            {
                _logger.LogInformation("Decorating tree with risk index");
                await _riskIndexer.DecorateTreeWithRiskIndex(tree);
            }

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