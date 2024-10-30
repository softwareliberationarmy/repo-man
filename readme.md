# repo-man

A console application that helps with understanding how healthy a codebase is. 

## OVERVIEW
repo-man contains several actions that can be run from the command line to help you understand the health of your codebase.
The first argument you pass to the executable should be the action you want to take (similar to the dotnet CLI usage pattern).

### diagram
```
.\repo-man.exe diagram --repo "C:\path\to\your\git\repo" --maxRadius 100 --outputDir "C:\path\to\output\folder" --background "white" --fileName "diagram.svg" --ignoreFileTypes "png|jpg|bmp"
```
The diagram action generates an SVG file visually representing the files in your git repository. 
Each circle represents a file, and the size of the circle is proportional to the size of the file. 
The color of the circle is determined by the file extension. 
Files are showing in the folder structure of the repository. Here is a diagram of this repository: 

![Visualization of this repo](./diagram.svg)

#### Config options

When you wish to generate a diagram, your first argument should be "diagram".
Below are additional configuration keys (case-sensitive) that can be passed at the command line:

- **repo (required)** - relative or absolute folder path to the git repository you want to diagram (the folder containing the .git folder)
- maxRadius - by default, diagrammed files have a radius range of 10 to 100. All of the file sizes in your repo are charted on that scale. If you have a wide disparity in file sizes, you may want to set the maxRadius lower so you can see the very small files better. Setting the maxRadius to 10 will chart all files the same size.
- outputDir - the directory to write the diagram.svg file. Defaults to the git repository root folder.
- background - sets the background color of the svg file. Defaults to "blanchedalmond" 🐿️.
- fileName - for setting the diagram file name, defaults to diagram.svg
- ignoreFileTypes - a pipe-delimited list of file extensions to ignore from analysis and from the diagram. For example, passing in "png|jpg|bmp" will ignore all image files from the diagram.

### review (alpha version)
```
.\repo-man.exe review --repo "C:\path\to\your\git\repo"
```
The review action pulls historical commit data from the git repository's current branch, and generates a text report to the command line
identifying code quality risks in the repository. The report is generated using AI to analyze the data, and is intended to be a starting
point for further investigation. The report is still early days, but with some prompt tuning could prove to be valuable. 

### Config options

The review action only requires that you provide a repo path that points to a valid git repository.
repo-man will analyze the repository's current branch and generate a report to the console. 
Right now you must have Ollama installed on your machine to run the analysis. 
The appSettings.json file contains settings for the local Ollama endpoint and the model to use for analysis.
In the future, this may be configured to be more LLM-agnostic and could point to a remote LLM API for better analysis. 

## DEPENDENCIES

- Visual Studio 2022
- .NET 8
- Nuget packages: libgit2sharp

## GETTING STARTED

The console project's launchSettings.json is configured to use its own git repo when you run the solution in debug mode, 
so you should be able to clone the repo and run it immediately.

If you want to run it from the command line, navigate to the folder with the executable and run it like this: 

## CONFIG OPTIONS

Because the tool is meant to be run from a command line or batch script, 
the primarily anticipated means of passing options is through command line args, 
but the Microsoft default host builder does set you up with the rest of the typical configuration sources.

When you call the tool from the command line, your first argument should always be the action you want to take, with no key.
Other configuration options should follow in the form of key-value pairs.


## TODO

- feature: change the intensity of the file color based on how recently it was modified if option passed in
- feature: change the intensity of the file color based on how many commits it has if option passed in

