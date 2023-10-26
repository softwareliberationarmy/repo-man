using System.Xml.Serialization;
using repo_man.domain.Git;

namespace repo_man.domain;

public class Tree
{
    public void AddFile(string filePath, long fileSize, Commit[] commits)
    {
        if (filePath.Contains("/"))
        {
            var segments = filePath.Split('/');
            var folderName = segments[0];
            Func<GitFolder, bool> matchingName = f => f.Name == folderName;

            if (!Folders.Any(matchingName))
            {
                AddFolder(folderName);
            }
            Folders.First(matchingName)
                .AddFile(segments.Skip(1).ToArray(), fileSize, commits);
        }
        else
        {
            _topLevelFiles.Add(new GitFile(filePath, fileSize, commits.AsReadOnly()));
        }
    }

    private void AddFolder(string folderName)
    {
        _folders.Add(new GitFolder(folderName));
    }

    private readonly List<GitFile> _topLevelFiles = new List<GitFile>();
    public IReadOnlyCollection<GitFile> TopLevelFiles => _topLevelFiles.AsReadOnly();

    private readonly List<GitFolder> _folders = new List<GitFolder>();
    public IReadOnlyCollection<GitFolder> Folders => _folders.AsReadOnly();
}

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
            var folderName = segments[0];
            Func<GitFolder, bool> matchingName = f => f.Name == folderName;
            if (!Folders.Any(matchingName))
            {
                AddFolder(folderName);
            }
            Folders.First(matchingName)
                .AddFile(segments.Skip(1).ToArray(), fileSize, commits);

        }
    }
    private void AddFolder(string folderName)
    {
        _folders.Add(new GitFolder(folderName));
    }
}