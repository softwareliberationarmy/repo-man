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
        var (startAt,topLevelMaxX) = WriteTopLevelFiles(tree, new StartingPoint(LeftMargin, TopMargin));
        _logger.LogInformation("Writing foldered files");
        var (maxX, maxY) = WriteFolderFiles(tree.Folders, startAt, startAt.X, tree);

        return new ChartData { Data = _stringBuilder.ToSvgString(), Size = new Point(Math.Max(maxX, topLevelMaxX), maxY)};
    }

    private (StartingPoint,int) WriteTopLevelFiles(GitTree tree, StartingPoint startAt)
    {
        const int topLevelFilesBottomMargin = 10;

        var (fileStart, fileMaxY) = WriteFiles(tree.Files, startAt, startAt.Y, topLevelFilesBottomMargin, tree);

        //done writing top-level files. Create a new starting point back at the left margin in a new row
        startAt = new StartingPoint(X: LeftMargin, Y: fileMaxY);
        return (startAt, fileStart.X);
    }

    private (int maxX, int maxY) WriteFolderFiles(IReadOnlyCollection<GitFolder> folders, StartingPoint startAt,
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
                new StartingPoint(X: folderStartAt.X + folderPadding, Y: folderStartAt.Y + folderPadding);

            (folderFileStartAt, maxY) = WriteFiles(folder.Files, folderFileStartAt, maxY, folderPadding, gitTree);
            maxX = Math.Max(maxX, folderFileStartAt.X);

            var folderBorderOffset = 0;
            if (folder.Folders.Any())
            {
                folderBorderOffset = 10;
                (maxX,maxY) = WriteFolderFiles(folder.Folders, new StartingPoint(folderStartAt.X + LeftMargin, folderStartAt.Y + TopMargin), maxX, gitTree);
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

        return (maxX, maxY);
    }

    private (StartingPoint folderFileStartAt, int maxY) WriteFiles(IReadOnlyCollection<GitFile> files,
        StartingPoint folderFileStartAt, int maxY, int bottomMargin, GitTree gitTree)
    {
        const int InterFileMargin = 5;
        foreach (var file in files.OrderByDescending(x => x.FileSize))
        {
            _logger.LogInformation(file.Name);
            var radius = _fileRadiusCalculator.CalculateFileRadius(file, gitTree);
            var y = folderFileStartAt.Y + radius;
            var x = folderFileStartAt.X + radius;

            _stringBuilder.AddFileCircle(x, y, radius, file.Name);

            maxY = Math.Max(maxY, y + radius + bottomMargin);
            folderFileStartAt = folderFileStartAt with { X = x + radius + InterFileMargin };
        }

        return (folderFileStartAt, maxY);
    }

    private sealed record StartingPoint(int X, int Y);

}