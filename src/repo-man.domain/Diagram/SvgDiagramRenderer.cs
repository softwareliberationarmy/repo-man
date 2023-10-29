using repo_man.domain.Git;


namespace repo_man.domain.Diagram
{
    public class SvgDiagramRenderer : IDiagramRenderer
    {

        public Task CreateDiagram(GitTree tree)
        {
            //TODO: determine the diagram type from the config, defaulting to by type
            //TODO: scrape the file extensions to build the legend
            //TODO: build the set of circles 

            throw new NotImplementedException();
        }
    }
}
