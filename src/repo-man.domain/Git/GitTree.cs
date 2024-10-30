namespace repo_man.domain.Git;

public class GitTree : GitFolder
{
    private long _smallestFile; 
    private long _largestFile;
    private int _minCommitCount;
    private int _maxCommitCount;

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

        if (_minCommitCount == 0 || commits.Length < _minCommitCount)
        {
            _minCommitCount = commits.Length;
        }

        if (_maxCommitCount == 0 || commits.Length > _maxCommitCount)
        {
            _maxCommitCount = commits.Length;
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

    public int GetMinCommitCount()
    {
        return _minCommitCount;
    }

    public int GetMaxCommitCount()
    {
        return _maxCommitCount;
    }
}