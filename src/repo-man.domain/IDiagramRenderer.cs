using repo_man.domain.Git;

namespace repo_man.domain;

public interface IDiagramRenderer
{
    Task RenderTree(Tree tree);
}