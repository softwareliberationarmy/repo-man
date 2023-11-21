# repo-man

A console application that generates diagrams helping users to visualize the codebase. The original idea was to compile this project and run the resulting executable as a husky pre-commit hook. So each time you committed changes to the repository, an updated diagram would be generated, showing you the relative size of the files in your project, like this:

![Visualization of this repo](./diagram.svg)

## DEPENDENCIES

- Visual Studio 2022
- .NET 7
- Nuget packages: libgit2sharp

## GETTING STARTED

The console project's launchSettings.json is configured to use its own git repo when you run the solution in debug mode, so you should be able to clone the repo and run it immediately.

## CONFIG OPTIONS

Because the tool is meant to be run from a command line or batch script, the primarily anticipated means of passing options is through command line args, but the Microsoft default host builder does set you up with the rest of the typical configuration sources.

Below are the configuration values:

- **repo (required)** - relative or absolute folder path to the git repository you want to diagram (the folder containing the .git folder)
- maxRadius - by default, diagrammed files have a radius range of 10 to 100. All of the file sizes in your repo are charted on that scale. If you have a wide disparity in file sizes, you may want to set the maxRadius lower so you can see the very small files better. Setting the maxRadius to 10 will chart all files the same size.
- outputDir - the directory to write the diagram.svg file. Defaults to the git repository root folder.
- background - sets the background color of the svg file. Defaults to "blanchedalmond" 🐿️.

## TODO

- feature: implement ignored file extensions for binary file types (png, jpg,
  bmp, etc.)
- feature: be able to specify a different diagram file name in the configuration
- performance: figure out how to crawl the repo for commits much faster than we're doing today (libgit2sharp crawls so slowly that we had to shut off commit gathering)
- feature: change the intensity of the file color based on how recently it was modified if option passed in
- feature: change the intensity of the file color based on how many commits it has if option passed in

## NOTES
