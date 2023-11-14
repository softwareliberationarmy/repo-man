using System.Drawing;
using Microsoft.Extensions.Logging;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class SvgBoxChartDataWriter : ISvgChartDataWriter
{
    //SVG file outer margins
    private const int TopMargin = 10;
    private const int LeftMargin = 10;

    private readonly SvgChartStringBuilder _stringBuilder;
    private readonly ILogger<SvgBoxChartDataWriter> _logger;

    public SvgBoxChartDataWriter(SvgChartStringBuilder stringBuilder, ILogger<SvgBoxChartDataWriter> logger)
    {
        _stringBuilder = stringBuilder;
        _logger = logger;
    }

    public ChartData WriteChartData(GitTree tree)
    {
        _logger.LogInformation("Writing top-level files");
        var (startAt,topLevelMaxX) = WriteTopLevelFiles(tree, new StartingPoint(LeftMargin, TopMargin));
        _logger.LogInformation("Writing foldered files");
        var (maxX, maxY) = WriteFolderFiles(tree.Folders, startAt, tree.GetMinFileSize(), startAt.X);

        return new ChartData { Data = _stringBuilder.ToSvgString(), Size = new Point(Math.Max(maxX, topLevelMaxX), maxY)};
    }

    private (StartingPoint,int) WriteTopLevelFiles(GitTree tree, StartingPoint startAt)
    {
        const int topLevelFilesBottomMargin = 10;

        var (fileStart, fileMaxY) = WriteFiles(tree.Files, startAt, startAt.Y, topLevelFilesBottomMargin, tree.GetMinFileSize());

        //done writing top-level files. Create a new starting point back at the left margin in a new row
        startAt = new StartingPoint(X: LeftMargin, Y: fileMaxY);
        return (startAt, fileStart.X);
    }

    private (int maxX, int maxY) WriteFolderFiles(IReadOnlyCollection<GitFolder> folders, StartingPoint startAt, long minFileSize, int maxX)
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

            (folderFileStartAt, maxY) = WriteFiles(folder.Files, folderFileStartAt, maxY, folderPadding, minFileSize);
            maxX = Math.Max(maxX, folderFileStartAt.X);

            var folderBorderOffset = 0;
            if (folder.Folders.Any())
            {
                folderBorderOffset = 10;
                (maxX,maxY) = WriteFolderFiles(folder.Folders, new StartingPoint(folderStartAt.X + LeftMargin, folderStartAt.Y + TopMargin), minFileSize, maxX);
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
        StartingPoint folderFileStartAt, int maxY, int bottomMargin, long minFileSize)
    {
        const int InterFileMargin = 5;
        foreach (var file in files.OrderByDescending(x => x.FileSize))
        {
            _logger.LogInformation(file.Name);
            var radius = CalculateFileRadius(minFileSize, file);
            var y = folderFileStartAt.Y + radius;
            var x = folderFileStartAt.X + radius;

            _stringBuilder.AddFileCircle(x, y, radius, file.Name);

            maxY = Math.Max(maxY, y + radius + bottomMargin);
            folderFileStartAt = folderFileStartAt with { X = x + radius + InterFileMargin };
        }

        return (folderFileStartAt, maxY);
    }

    private sealed record StartingPoint(int X, int Y);


    private static int CalculateFileRadius(long minFileSize, GitFile file)
    {
        const int minRadius = 10;
        var radius = (int)(file.FileSize / minFileSize) * minRadius;
        return radius;
    }
}