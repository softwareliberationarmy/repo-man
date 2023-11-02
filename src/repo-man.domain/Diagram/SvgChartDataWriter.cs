using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class SvgChartDataWriter
{
    //SVG file outer margins
    private const int TopMargin = 10;
    private const int LeftMargin = 10;

    //inner padding and margins
    private const int FileMargin = 5;
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
        WriteFolderFiles(tree, startAt);

        return new ChartData { Data = _stringBuilder.ToSvgString() };
    }

    private StartingPoint WriteTopLevelFiles(GitTree tree, StartingPoint startAt)
    {
        const long topLevelFilesBottomMargin = 10;

        long fileMaxY = startAt.Y;
        foreach (var file in tree.Files)
        {
            var radius = (file.FileSize / tree.GetMinFileSize()) * MinRadius;
            var y = startAt.Y + radius;
            var x = startAt.X + radius;

            _stringBuilder.AddFileCircle(x, y, radius, file.Name);

            fileMaxY = Math.Max(fileMaxY, y + radius + topLevelFilesBottomMargin);
            startAt = startAt with { X = x + radius + FileMargin };
        }

        //done writing top-level files. Create a new starting point back at the left margin in a new row
        startAt = new StartingPoint(X: LeftMargin, Y: fileMaxY);

        return startAt;
    }

    private void WriteFolderFiles(GitTree tree, StartingPoint startAt)
    {
        long maxY = startAt.Y;
        foreach (var folder in tree.Folders)
        {
            var folderStartAt = startAt;
            var folderFileStartAt =
                new StartingPoint(X: folderStartAt.X + FolderPadding, Y: folderStartAt.Y + FolderPadding);

            foreach (var file in folder.Files)
            {
                var radius = (file.FileSize / tree.GetMinFileSize()) * MinRadius;
                var y = folderFileStartAt.Y + radius;
                var x = folderFileStartAt.X + radius;

                _stringBuilder.AddFileCircle(x, y, radius, file.Name);
                maxY = Math.Max(maxY, y + radius + FolderPadding);
                folderFileStartAt = folderFileStartAt with { X = x + radius + FileMargin };
            }

            var width = folderFileStartAt.X - folderStartAt.X;
            var height = maxY - folderStartAt.Y;
            var rectangleX = folderStartAt.X;
            var rectangleY = folderStartAt.Y;
            var folderName = folder.Name;
            _stringBuilder.AddBoundingRectangle(rectangleX, rectangleY, width, height, folderName);
        }
    }

    private record StartingPoint(long X, long Y);
}