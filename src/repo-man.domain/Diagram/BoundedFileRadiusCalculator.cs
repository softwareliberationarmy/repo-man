using Microsoft.Extensions.Configuration;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class BoundedFileRadiusCalculator : IFileRadiusCalculator
{
    private readonly IConfiguration _config;

    public BoundedFileRadiusCalculator(IConfiguration config)
    {
        _config = config;
    }

    public int CalculateFileRadius(GitFile file, GitTree gitTree)
    {
        const int minRadius = 10;
        var maxRadius = 100;
        if (_config["maxRadius"] is { } maxRadiusString 
            && int.TryParse(maxRadiusString, out var newMaxRadius) 
            && newMaxRadius >= minRadius)
        {
            maxRadius = newMaxRadius;
        }

        var minFileSize = gitTree.GetMinFileSize();
        var maxFileSize = gitTree.GetMaxFileSize();

        if (file.FileSize == minFileSize)
        {
            return minRadius;
        }

        //NOTE: should never get a divide by zero exception due to check above
        var percent = (double)(file.FileSize - minFileSize) / (maxFileSize - minFileSize);
        var increment = (int)Math.Round(percent * (maxRadius - minRadius));
        var fileRadius = minRadius + increment;

        return fileRadius;
    }
}