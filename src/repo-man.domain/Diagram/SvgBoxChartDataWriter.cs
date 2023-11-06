﻿using System.Drawing;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class SvgBoxChartDataWriter : ISvgBoxChartDataWriter
{
    //SVG file outer margins
    private const int TopMargin = 10;
    private const int LeftMargin = 10;

    private readonly SvgStringBuilder _stringBuilder;

    public SvgBoxChartDataWriter(SvgStringBuilder stringBuilder)
    {
        _stringBuilder = stringBuilder;
    }

    public ChartData WriteChartData(GitTree tree)
    {
        var (startAt,topLevelMaxX) = WriteTopLevelFiles(tree, new StartingPoint(LeftMargin, TopMargin));
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
        const int minRadius = 10;
        foreach (var file in files.OrderByDescending(x => x.FileSize))
        {
            var radius = (int)(file.FileSize / minFileSize) * minRadius;
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