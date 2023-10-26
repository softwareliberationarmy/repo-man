using repo_man.domain.Git;

namespace repo_man.domain;

public class Tree
{
    public void AddFile(string filePath, long FileSize, Commit[] commits)
    {
        _topLevelFiles.Add(new GitFile(filePath, FileSize, commits.AsReadOnly()));
    }

    private readonly List<GitFile> _topLevelFiles = new List<GitFile>();
    public IReadOnlyCollection<GitFile> TopLevelFiles => _topLevelFiles.AsReadOnly();
}