namespace repo_man.domain.Git;

public class Tree : GitFolder
{
    public Tree() : base("")
    {
    }

    public void AddFile(string filePath, long fileSize, Commit[] commits)
    {
        AddFile(filePath.Split('/'), fileSize, commits);
    }

}