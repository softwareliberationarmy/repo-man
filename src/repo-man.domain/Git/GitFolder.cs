namespace repo_man.domain.Git;

public class GitFolder
{
    public string Name { get; }

    public GitFolder(string name)
    {
        Name = name;
    }

    private readonly List<GitFile> _files = new List<GitFile>();
    public IReadOnlyCollection<GitFile> Files => _files.AsReadOnly();

    private readonly List<GitFolder> _folders = new List<GitFolder>();
    public IReadOnlyCollection<GitFolder> Folders => _folders.AsReadOnly();

    public void AddFile(string[] segments, long fileSize, Commit[] commits)
    {
        if (segments.Length == 1)
        {
            _files.Add(new GitFile(segments[0], fileSize, commits.AsReadOnly()));
        }
        else if (segments.Length > 1)
        {
            AddToFolders(segments, fileSize, commits);
        }
    }

    private void AddToFolders(string[] segments, long fileSize, Commit[] commits)
    {
        var gitFolder = GetGitFolder(segments[0]);
        gitFolder.AddFile(segments.Skip(1).ToArray(), fileSize, commits);
    }

    private GitFolder GetGitFolder(string folderName)
    {
        Func<GitFolder, bool> matchingName = f => f.Name == folderName;
        if (!Folders.Any(matchingName))
        {
            AddFolder(folderName);
        }

        var gitFolder = Folders.First(matchingName);
        return gitFolder;
    }

    private void AddFolder(string folderName)
    {
        _folders.Add(new GitFolder(folderName));
    }
}