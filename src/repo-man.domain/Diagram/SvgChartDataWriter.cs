using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class SvgChartDataWriter
{
    //SVG file outer margins
    private const int TopMargin = 10;
    private const int LeftMargin = 10;

    //inner padding and margins
    private const int InterFileMargin = 5;
    private const int FolderPadding = 5;

    //other constants
    private const int MinRadius = 10;

    private readonly SvgStringBuilder _stringBuilder;

    public SvgChartDataWriter(SvgStringBuilder stringBuilder)
    {
        _stringBuilder = stringBuilder;
    }

    public ChartData WriteChartData(GitTree tree)
    {
        var startAt = WriteTopLevelFiles(tree, new StartingPoint(LeftMargin, TopMargin));
        WriteFolderFiles(tree.Folders, startAt, tree.GetMinFileSize());

        return new ChartData { Data = _stringBuilder.ToSvgString() };
    }

    private StartingPoint WriteTopLevelFiles(GitTree tree, StartingPoint startAt)
    {
        const int topLevelFilesBottomMargin = 10;

        long fileMaxY = startAt.Y;

        (_, fileMaxY) = WriteFiles(tree.Files, startAt, fileMaxY, topLevelFilesBottomMargin, tree.GetMinFileSize());

        //done writing top-level files. Create a new starting point back at the left margin in a new row
        startAt = new StartingPoint(X: LeftMargin, Y: fileMaxY);
        return startAt;
    }

    private void WriteFolderFiles(IReadOnlyCollection<GitFolder> folders, StartingPoint startAt, long minFileSize)
    {
        const long folderBottomMargin = 10;
        long maxY = startAt.Y;
        var folderStartAt = startAt;
        foreach (var folder in folders)
        {
            var folderFileStartAt =
                new StartingPoint(X: folderStartAt.X + FolderPadding, Y: folderStartAt.Y + FolderPadding);

            (folderFileStartAt, maxY) = WriteFiles(folder.Files, folderFileStartAt, maxY, FolderPadding, minFileSize);

            var width = folderFileStartAt.X - folderStartAt.X;
            var height = maxY - folderStartAt.Y;
            var rectangleX = folderStartAt.X;
            var rectangleY = folderStartAt.Y;
            var folderName = folder.Name;
            _stringBuilder.AddBoundingRectangle(rectangleX, rectangleY, width, height, folderName);

            folderStartAt = startAt with { Y = rectangleY + height + folderBottomMargin };
        }
    }

    private (StartingPoint folderFileStartAt, long maxY) WriteFiles(IReadOnlyCollection<GitFile> files, 
        StartingPoint folderFileStartAt, long maxY, int bottomMargin, long minFileSize)
    {
        foreach (var file in files.OrderByDescending(x => x.FileSize))
        {
            var radius = (file.FileSize / minFileSize) * MinRadius;
            var y = folderFileStartAt.Y + radius;
            var x = folderFileStartAt.X + radius;

            _stringBuilder.AddFileCircle(x, y, radius, file.Name);

            maxY = Math.Max(maxY, y + radius + bottomMargin);
            folderFileStartAt = folderFileStartAt with { X = x + radius + InterFileMargin };
        }

        return (folderFileStartAt, maxY);
    }

    private sealed record StartingPoint(long X, long Y);
}