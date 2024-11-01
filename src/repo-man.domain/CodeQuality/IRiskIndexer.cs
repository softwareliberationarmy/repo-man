using repo_man.domain.Git;

namespace repo_man.domain.CodeQuality;

public interface IRiskIndexer
{
    Task DecorateTreeWithRiskIndex(GitTree tree);
}