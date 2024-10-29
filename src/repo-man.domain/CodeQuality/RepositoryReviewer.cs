using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using repo_man.domain.Git;

namespace repo_man.domain.CodeQuality;

public class RepositoryReviewer
{
    private readonly ICodeQualityAnalyst _codeQualityAnalyst;
    private readonly ILogger<RepositoryReviewer> _logger;
    private readonly IGitRepoCrawler _repoCrawler;

    public RepositoryReviewer(ILogger<RepositoryReviewer> logger, IGitRepoCrawler repoCrawler, ICodeQualityAnalyst codeQualityAnalyst)
    {
        _codeQualityAnalyst = codeQualityAnalyst;
        _logger = logger;
        _repoCrawler = repoCrawler;
    }

    public virtual async Task ReviewCodeQuality()
    {
        _logger.LogInformation("Starting code quality review");

        _logger.LogInformation("Collecting git commit history");
        var commits = _repoCrawler.GitThemFiles();

        _logger.LogInformation("Requesting review from code quality analyst");
        var result = await _codeQualityAnalyst.EvaluateCodeQuality(commits);
        
        _logger.LogInformation("Code quality review complete. Result: \r\n{result}", result);

        //TODO: write review results to file
        //TODO: add sonarqube metrics

    }
}