using repo_man.domain.Git;

namespace repo_man.domain.Diagram.FileRadiusCalculator;

public interface IFileRadiusCalculator
{
    int CalculateFileRadius(GitFile file, GitTree gitTree);
}