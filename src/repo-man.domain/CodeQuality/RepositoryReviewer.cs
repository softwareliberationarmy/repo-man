using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using repo_man.domain.Git;

namespace repo_man.domain.CodeQuality;

public class RepositoryReviewer
{
    private readonly ITreeExtracter _treeExtracter;
    private readonly ICodeQualityAnalyst _codeQualityAnalyst;
    private readonly ILogger<RepositoryReviewer> _logger;

    public RepositoryReviewer(ITreeExtracter treeExtracter, ICodeQualityAnalyst codeQualityAnalyst, ILogger<RepositoryReviewer> logger)
    {
        _treeExtracter = treeExtracter;
        _codeQualityAnalyst = codeQualityAnalyst;
        _logger = logger;
    }

    public virtual async Task ReviewCodeQuality()
    {
        _logger.LogInformation("Starting code quality review");

        _logger.LogInformation("Collecting git commit history");
        var tree = _treeExtracter.GetFileTree();
        
        _logger.LogInformation("Requesting review from code quality analyst");
        var result = await _codeQualityAnalyst.EvaluateCodeQuality(tree);
        
        _logger.LogInformation("Code quality review complete. Result: \r\n{result}", result);
        //TODO: write review results to file
        //TODO: add sonarqube metrics

    }
}