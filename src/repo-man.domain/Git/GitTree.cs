namespace repo_man.domain.Git;

public class GitTree : GitFolder
{
    public GitTree() : base("")
    {
    }

    public void AddFile(string filePath, long fileSize, Commit[] commits)
    {
        AddFile(filePath.Split('/'), fileSize, commits);
    }

}