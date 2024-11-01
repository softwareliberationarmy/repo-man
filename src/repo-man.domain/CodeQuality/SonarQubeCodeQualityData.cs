namespace repo_man.domain.CodeQuality;

public class SonarQubeCodeQualityData
{
    public string? FilePath { get; set; }
    public long FileSizeInBytes { get; set; }
    public long GitCommitCount { get; set; }
    public long CyclomaticComplexity { get; set; }
    public long CodeSmells { get; set; }
    public long CriticalViolations { get; set; }
    public long MajorViolations { get; set; }
    public long AllViolations { get; set; }
}