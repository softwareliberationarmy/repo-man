namespace repo_man.domain;

public interface IDiagramRenderer
{
    Task RenderTree(Tree tree);
}