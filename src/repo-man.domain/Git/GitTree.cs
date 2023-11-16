namespace repo_man.domain.Git;

public class GitTree : GitFolder
{
    private long _smallestFile; 
    private long _largestFile;

    public GitTree() : base("")
    {
    }

    public void AddFile(string filePath, long fileSize, Commit[] commits)
    {
        if (_smallestFile == 0L || fileSize < _smallestFile)
        {
            _smallestFile = fileSize;
        }

        if (_largestFile == 0L || fileSize > _largestFile)
        {
            _largestFile = fileSize;
        }
        AddFile(filePath.Split('/'), fileSize, commits);
    }

    public long GetMinFileSize()
    {
        return _smallestFile;
    }

    public long GetMaxFileSize()
    {
        return _largestFile;
    }
}