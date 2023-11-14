using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public interface IFileRadiusCalculator
{
    int CalculateFileRadius(GitFile file);
}

public class UnboundedFileRadiusCalculator : IFileRadiusCalculator
{
    private readonly long _minFileSize;

    public UnboundedFileRadiusCalculator(GitTree tree)
    {
        _minFileSize = tree.GetMinFileSize();
    }

    public int CalculateFileRadius(GitFile file)
    {
        const int minRadius = 10;
        var radius = (int)(file.FileSize / _minFileSize) * minRadius;
        return radius;
    }
}