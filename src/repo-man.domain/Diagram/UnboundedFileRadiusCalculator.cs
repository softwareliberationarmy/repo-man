using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public interface IFileRadiusCalculator
{
    int CalculateFileRadius(GitFile file, GitTree gitTree);
}

public class UnboundedFileRadiusCalculator : IFileRadiusCalculator
{
    public int CalculateFileRadius(GitFile file, GitTree gitTree)
    {
        const int minRadius = 10;
        var radius = (int)(file.FileSize / gitTree.GetMinFileSize()) * minRadius;
        return radius;
    }
}