namespace repo_man.domain.Git;

public class GitFile(string Name, long FileSize, IReadOnlyCollection<Commit> Commits, string FullPath)
{
    public string Name { get; init; } = Name;
    public long FileSize { get; init; } = FileSize;
    public IReadOnlyCollection<Commit> Commits { get; init; } = Commits;
    public int RiskIndex { get; set; }
    public string FullPath { get; set; } = FullPath;
}