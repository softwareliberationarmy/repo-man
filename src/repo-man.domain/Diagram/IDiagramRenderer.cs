using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public interface IDiagramRenderer
{
    Task RenderTree(Tree tree);
}