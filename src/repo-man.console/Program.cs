using repo_man.console;
using repo_man.domain;

var visualizer = Bootstrapper.InitializeToTopLevelService<RepositoryVisualizer>(args);
await visualizer.GenerateDiagram();


//TO DO LIST: 
// implement IFileColorMapper
// define SvgLegendDataWriter
// extract interface from SvgChartDataWriter
// test the composition of the SvgDiagramRenderer now that we know where we're going with this
// add infrastructure class to write data to file (thin wrapper)
// define SvgComposer or use SvgDiagramRenderer as composer and add an SvgBookendManager
// add logging to the SvgChartDataWriter
// add logging to LibGit2SharpGitRepoCrawler
// add logging to GitRepositoryTreeExtracter
// feature: user can specify background color
