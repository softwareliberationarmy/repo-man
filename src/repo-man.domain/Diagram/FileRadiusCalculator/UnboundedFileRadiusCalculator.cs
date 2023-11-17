using repo_man.domain.Git;

namespace repo_man.domain.Diagram.FileRadiusCalculator;

/// <summary>
/// Calculates the radius of the circle based on the file size. The smallest file in the repository gets a radius of 10. All other files get a proportional radius.
/// The limit to the maximum radius size is unbounded, hence the class name. 
/// </summary>
public class UnboundedFileRadiusCalculator : IFileRadiusCalculator
{
    public int CalculateFileRadius(GitFile file, GitTree gitTree)
    {
        const int minRadius = 10;
        var radius = (int)(file.FileSize / gitTree.GetMinFileSize()) * minRadius;
        return radius;
    }
}