using Microsoft.Extensions.Configuration;
using repo_man.domain.Diagram.Calculators;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram.FileRadiusCalculator;

public class BoundedFileRadiusCalculator : IFileRadiusCalculator
{
    private readonly IConfiguration _config;
    private readonly BoundedIntCalculator _innerCalculator;

    public BoundedFileRadiusCalculator(IConfiguration config, BoundedIntCalculator innerCalculator)
    {
        _config = config;
        _innerCalculator = innerCalculator;
    }

    public int CalculateFileRadius(GitFile file, GitTree gitTree)
    {
        if (!_innerCalculator.IsInitialized)
        {
            const int minRadius = 10;
            var maxRadius = 100;
            if (_config["maxRadius"] is { } maxRadiusString
                && int.TryParse(maxRadiusString, out var newMaxRadius)
                && newMaxRadius >= minRadius)
            {
                maxRadius = newMaxRadius;
            }

            var minFileRadius = (int)Math.Round(Math.Sqrt(gitTree.GetMinFileSize()));
            var maxFileRadius = (int)Math.Round(Math.Sqrt(gitTree.GetMaxFileSize()));
            _innerCalculator.SetBounds(minFileRadius, maxFileRadius, minRadius, maxRadius);
        }

        return _innerCalculator.Calculate((int)Math.Round(Math.Sqrt(file.FileSize)));
    }
}