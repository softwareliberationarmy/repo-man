using System.Drawing;
using Microsoft.Extensions.Logging;
using repo_man.domain.Diagram.FileRadiusCalculator;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class SvgBoxChartDataWriter : ISvgChartDataWriter
{
    //SVG file outer margins
    private const int TopMargin = 10;
    private const int LeftMargin = 10;

    private readonly SvgChartStringBuilder _stringBuilder;
    private readonly ILogger<SvgBoxChartDataWriter> _logger;
    private readonly IFileRadiusCalculator _fileRadiusCalculator;

    public SvgBoxChartDataWriter(SvgChartStringBuilder stringBuilder, ILogger<SvgBoxChartDataWriter> logger, IFileRadiusCalculator fileRadiusCalculator)
    {
        _stringBuilder = stringBuilder;
        _logger = logger;
        _fileRadiusCalculator = fileRadiusCalculator;
    }

    public ChartData WriteChartData(GitTree tree)
    {
        _logger.LogInformation("Writing top-level files");
        const int topLevelFilesBottomMargin = 10;
        var startAt = new Point(LeftMargin, TopMargin);
        var newMaxPoint = WriteFiles(tree.Files, startAt, TopMargin, topLevelFilesBottomMargin, tree);

        _logger.LogInformation("Writing foldered files");
        var folderMaxPoint = WriteFolderFiles(tree.Folders, newMaxPoint with { X = LeftMargin }, startAt.X, tree);

        return new ChartData { Data = _stringBuilder.ToSvgString(), Size = folderMaxPoint with { X = Math.Max(folderMaxPoint.X, newMaxPoint.X) } };
    }

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
            var folderFileStartAt =
                new Point(folderStartAt.X + folderPadding, folderStartAt.Y + folderPadding);

            var newMaxPoint = WriteFiles(folder.Files, folderFileStartAt, maxY, folderPadding, gitTree);
            maxY = newMaxPoint.Y;

            maxX = Math.Max(maxX, newMaxPoint.X);

            var folderBorderOffset = 0;
            if (folder.Folders.Any())
            {
                folderBorderOffset = 10;
                var localMax = WriteFolderFiles(folder.Folders,
                    new Point(folderStartAt.X + LeftMargin, folderStartAt.Y + TopMargin), maxX, gitTree);
                maxX = localMax.X;
                maxY = localMax.Y;
            }

            var width = maxX + folderBorderOffset - folderStartAt.X;
            var height = maxY + folderBorderOffset - folderStartAt.Y;
            var rectangleX = folderStartAt.X;
            var rectangleY = folderStartAt.Y;
            var folderName = folder.Name;
            _stringBuilder.AddBoundingRectangle(rectangleX, rectangleY, width, height, folderName);

            maxX = Math.Max(maxX, rectangleX + width);
            maxY = Math.Max(maxY, rectangleY + height);
            folderStartAt = startAt with { Y = rectangleY + height + folderBottomMargin };
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

            _stringBuilder.AddFileCircle(x, y, radius, file.Name);

            maxY = Math.Max(maxY, y + radius);
            startingPoint = startingPoint with { X = x + radius + InterFileMargin };
        }

        return startingPoint with { Y = maxY + bottomMargin };

    }
}
