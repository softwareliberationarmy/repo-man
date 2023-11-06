# repo-man

A console application that generates diagrams helping users to visualize the codebase

## TODO
* implement IFileColorMapper
* define SvgLegendDataWriter
* test the composition of the SvgDiagramRenderer now that we know where we're going with this
* add infrastructure class to write data to file (thin wrapper)
* define SvgComposer or use SvgDiagramRenderer as composer and add an SvgBookendManager
* add logging to the SvgChartDataWriter
* add logging to LibGit2SharpGitRepoCrawler
* feature: user can specify background color