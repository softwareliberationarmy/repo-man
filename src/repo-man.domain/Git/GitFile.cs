namespace repo_man.domain.Git;

public class GitFile(string Name, long FileSize, IReadOnlyCollection<Commit> Commits, string FullPath)
{
    public string Name { get; init; } = Name;
    public long FileSize { get; init; } = FileSize;
    public IReadOnlyCollection<Commit> Commits { get; init; } = Commits;
    public string FullPath { get; init; } = FullPath;
    public int RiskIndex { get; set; }
    public string ToolTip { get; set; } = $"{FullPath}\r\n\r\nFile size (bytes): {FileSize}\r\n# of commits: {Commits.Count}";
    public string Label { get; set; } = Name;
}