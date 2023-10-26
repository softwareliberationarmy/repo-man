using System.Xml.Serialization;
using repo_man.domain.Git;

namespace repo_man.domain;

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