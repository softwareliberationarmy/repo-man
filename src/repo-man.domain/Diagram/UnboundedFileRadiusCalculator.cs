using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class UnboundedFileRadiusCalculator
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