using repo_man.domain.Git;

namespace repo_man.domain.CodeQuality;

public class SonarQubeRiskIndexer: IRiskIndexer
{
    private readonly ICodeQualityDataSource<SonarQubeCodeQualityData> _sonarQubeApi;
    private readonly RiskIndexCalculator<SonarQubeCodeQualityData> _riskIndexCalculator;

    public SonarQubeRiskIndexer(ICodeQualityDataSource<SonarQubeCodeQualityData> sonarQubeApi, RiskIndexCalculator<SonarQubeCodeQualityData> riskIndexCalculator)
    {
        _sonarQubeApi = sonarQubeApi;
        _riskIndexCalculator = riskIndexCalculator;
    }

    public async Task DecorateTreeWithRiskIndex(GitTree tree)
    {
        var metrics = new List<Metric<SonarQubeCodeQualityData>>
        {
            new(d => d.FileSizeInBytes, tree.GetMaxFileSize(), 0.05, nameof(SonarQubeCodeQualityData.FileSizeInBytes)),
            new(d => d.GitCommitCount, tree.GetMaxCommitCount(), 0.15, nameof(SonarQubeCodeQualityData.GitCommitCount))
        };

        var data = await _sonarQubeApi.GetCodeQualityData();

        metrics.Add(new(x => x.CyclomaticComplexity, data.Max(d => d.CyclomaticComplexity), 0.20, nameof(SonarQubeCodeQualityData.CyclomaticComplexity)));
        metrics.Add(new(x => x.CodeSmells, data.Max(d => d.CodeSmells), 0.10, nameof(SonarQubeCodeQualityData.CodeSmells)));
        metrics.Add(new(x => x.CriticalViolations, data.Max(d => d.CriticalViolations), 0.25, nameof(SonarQubeCodeQualityData.CriticalViolations)));
        metrics.Add(new(x => x.MajorViolations, data.Max(d => d.MajorViolations), 0.15, nameof(SonarQubeCodeQualityData.MajorViolations)));
        metrics.Add(new(x => x.AllViolations, data.Max(d => d.AllViolations), 0.10, nameof(SonarQubeCodeQualityData.AllViolations)));

        foreach (var file in tree.GetAllFiles())
        {
            var sonarQubeData = data.FirstOrDefault(x => x.FilePath == file.FullPath);
            if (sonarQubeData != null)
            {
                var riskIndex = _riskIndexCalculator.CalculateRiskIndex(metrics, sonarQubeData);
                file.RiskIndex = riskIndex;
            }
        }
    }
}