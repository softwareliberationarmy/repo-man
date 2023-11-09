# repo-man

A console application that generates diagrams helping users to visualize the codebase

## TODO
=> implement IFileColorMapper
* add logic for files with no extension to return the full file name
* define SvgLegendDataWriter
* test the composition of the SvgDiagramRenderer now that we know where we're going with this
* add infrastructure class to write data to file (thin wrapper)
* define SvgComposer or use SvgDiagramRenderer as composer and add an SvgBookendManager
* add logging to the SvgChartDataWriter
* feature: user can specify background color
* feature: implement ignored file extensions for binary file types (png, jpg, bmp, etc.)
* 

## NOTES
* Rather than trying to capture every single file extension, let's only include the file extensions we're concerned about (known code file extensions)
* 
