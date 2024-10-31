using System.Drawing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using repo_man.domain.Diagram.Calculators;
using repo_man.domain.Diagram.FileRadiusCalculator;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class SvgBoxChartDataWriter : ISvgChartDataWriter
{
    //SVG file outer margins
    private const int TopMargin = 10;
    private const int LeftMargin = 10;

    private bool _isRiskChart;

    private readonly SvgChartStringBuilder _stringBuilder;
    private readonly ILogger<SvgBoxChartDataWriter> _logger;
    private readonly IFileRadiusCalculator _fileRadiusCalculator;
    private readonly BoundedIntCalculator _colorIntensityCalculator;

    public SvgBoxChartDataWriter(SvgChartStringBuilder stringBuilder, ILogger<SvgBoxChartDataWriter> logger,
        IFileRadiusCalculator fileRadiusCalculator, BoundedIntCalculator colorIntensityCalculator, IConfiguration config)
    {
        _stringBuilder = stringBuilder;
        _logger = logger;
        _fileRadiusCalculator = fileRadiusCalculator;
        _colorIntensityCalculator = colorIntensityCalculator;

        _isRiskChart = config["type"] == "risk";
    }

    public ChartData WriteChartData(GitTree tree)
    {
        SetColorIntensityBounds(tree);
        _logger.LogInformation("Writing top-level files");
        const int topLevelFilesBottomMargin = 10;
        var startAt = new Point(LeftMargin, TopMargin);
        var newMaxPoint = WriteFiles(tree.Files, startAt, TopMargin, topLevelFilesBottomMargin, tree);

        _logger.LogInformation("Writing foldered files");
        var folderMaxPoint = WriteFolderFiles(tree.Folders, newMaxPoint with { X = LeftMargin }, startAt.X, tree);

        return new ChartData { Data = _stringBuilder.ToSvgString(), Size = folderMaxPoint with { X = Math.Max(folderMaxPoint.X, newMaxPoint.X) } };
    }

    private void SetColorIntensityBounds(GitTree tree)
    {
        if (_isRiskChart)
        {
            _colorIntensityCalculator.SetBounds(0, 100, 20, 100);
        }
        else
        {
            if (tree.GetMinCommitCount() == 0 && tree.GetMaxCommitCount() == 0)
            {
                //no commits at all, so let all circles have the full intensity
                _colorIntensityCalculator.SetBounds(0, 0, 100, 100);
            }
            else
            {
                _colorIntensityCalculator.SetBounds(tree.GetMinCommitCount(), tree.GetMaxCommitCount(), 20, 100);
            }

        }
    }

    /// <summary>
    /// Given a collection of Git folders, a starting point, a current max X value, and the whole Git tree:
    /// loop through the folders and:
    ///     write the files in the folder to the svg string, returning a new max point
    ///     if files were written 
    ///         set the new maxY to the returned max Y
    ///         set the current maxX to the returned max X if it is larger
    ///     if there are any subfolders
    ///         recurse, passing in the subfolders, a starting point indented top left, the current max X, and the whole git tree
    ///     calculate the width of the bounding rectangle for this folder
    ///         width = the current max X minus the starting point + 10 if there are subfolders
    ///         height = the current max Y minus the starting point + 10 if there are subfolders
    /// </summary>
    /// <param name="folders"></param>
    /// <param name="startAt"></param>
    /// <param name="maxX"></param>
    /// <param name="gitTree"></param>
    /// <returns></returns>
    private Point WriteFolderFiles(IReadOnlyCollection<GitFolder> folders, Point startAt,
        int maxX, GitTree gitTree)
    {
        const int folderPadding = 5;
        const int folderBottomMargin = 10;
        int maxY = startAt.Y;
        var folderStartAt = startAt;
        foreach (var folder in folders)
        {
            _logger.LogInformation("{folderName}/", folder.Name);

            var folderFilesStartingPoint =
                new Point(folderStartAt.X + folderPadding, folderStartAt.Y + folderPadding);
            var newMaxPoint = WriteFiles(folder.Files, folderFilesStartingPoint, maxY, folderPadding, gitTree);
            if (folderFilesStartingPoint != newMaxPoint)
            {
                maxY = newMaxPoint.Y;
                maxX = Math.Max(maxX, newMaxPoint.X);
            }

            var folderBorderOffset = 0;
            if (folder.Folders.Any())
            {
                if (folder.Files.Any())
                {
                    maxY -= 5;
                }
                folderBorderOffset = 10;
                var localMax = WriteFolderFiles(folder.Folders,
                    new Point(folderStartAt.X + LeftMargin, maxY + TopMargin), maxX, gitTree);
                maxX = Math.Max(maxX, localMax.X);
                maxY = localMax.Y;
            }

            var width = maxX + folderBorderOffset - folderStartAt.X;
            var height = maxY + folderBorderOffset - folderStartAt.Y;
            _stringBuilder.AddBoundingRectangle(folderStartAt.X, folderStartAt.Y, width, height, folder.Name);

            maxX = Math.Max(maxX, folderStartAt.X + width);
            maxY = Math.Max(maxY, folderStartAt.Y + height);
            folderStartAt = startAt with { Y = folderStartAt.Y + height + folderBottomMargin };
        }

        return new Point(maxX, maxY);
    }

    /// <summary>
    /// Given some files, a tree, a starting point, a current max Y, and a bottom margin:
    /// if no files, return starting point
    /// else
    ///     loop through the files ordered by size descending:
    ///         calculate a file radius
    ///         set the x and y coordinates of the circle center
    ///         add the circle to the svg string
    ///         calculate a new maxY
    ///         calculate a new starting point for the next file
    ///     return the furthest point touched (including the bottom margin)
    /// </summary>
    /// <param name="files">the set of files to write sequentially</param>
    /// <param name="startingPoint">the point at which to begin drawing the circles</param>
    /// <param name="maxY">the current max Y value</param>
    /// <param name="bottomMargin">the bottom margin that should be added below the files</param>
    /// <param name="gitTree">the entire tree of git files</param>
    /// <returns>the maximum x and y values reached when writing these files</returns>
    private Point WriteFiles(IReadOnlyCollection<GitFile> files,
        Point startingPoint, int maxY, int bottomMargin, GitTree gitTree)
    {
        if (!files.Any()) return startingPoint;

        const int InterFileMargin = 5;
        foreach (var file in files.OrderByDescending(x => x.FileSize))
        {
            _logger.LogInformation(file.Name);
            var radius = _fileRadiusCalculator.CalculateFileRadius(file, gitTree);
            var y = startingPoint.Y + radius;
            var x = startingPoint.X + radius;

            var intensity = _colorIntensityCalculator.Calculate(_isRiskChart ? file.RiskIndex : file.Commits.Count);

            _stringBuilder.AddFileCircle(x, y, radius, file.Name, intensity);

            maxY = Math.Max(maxY, y + radius);
            startingPoint = startingPoint with { X = x + radius + InterFileMargin };
        }

        return startingPoint with { Y = maxY + bottomMargin };

    }
}
