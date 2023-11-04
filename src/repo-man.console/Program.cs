using repo_man.console;
using repo_man.domain;

var visualizer = Bootstrapper.InitializeToTopLevelService<RepositoryVisualizer>(args);
await visualizer.GenerateDiagram();

