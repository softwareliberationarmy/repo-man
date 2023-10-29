namespace repo_man.domain.Git;

public class GitTree : GitFolder
{
    private long _smallestFile; 

    public GitTree() : base("")
    {
    }

    public void AddFile(string filePath, long fileSize, Commit[] commits)
    {
        if (_smallestFile == 0L || fileSize < _smallestFile)
        {
            _smallestFile = fileSize;
        }
        AddFile(filePath.Split('/'), fileSize, commits);
    }

    public long GetMinFileSize()
    {
        return _smallestFile;
    }
}