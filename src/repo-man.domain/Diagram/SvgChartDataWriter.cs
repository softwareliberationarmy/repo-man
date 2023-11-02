using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class SvgChartDataWriter
{
    private readonly SvgStringBuilder _stringBuilder;

    public SvgChartDataWriter(SvgStringBuilder stringBuilder)
    {
        _stringBuilder = stringBuilder;
    }

    public ChartData WriteChartData(GitTree tree)
    {
        var filePadding = 5;
        var minFileSize = tree.GetMinFileSize();

        var startAt = new StartingPoint(10, 10);
        long maxY = 10;
        foreach (var file in tree.Files)
        {
            var radius = (file.FileSize / minFileSize) * 10;
            var y = startAt.Y + radius;
            var x = startAt.X + radius;

            _stringBuilder.AddFileCircle(x, y, radius, file.Name);
            maxY = Math.Max(maxY, y + radius + 10);
            startAt = startAt with { X = x + radius + filePadding };
        }

        startAt = new StartingPoint(X: 10, Y: maxY);

        foreach (var folder in tree.Folders)
        {
            var folderPadding = 5;
            var folderStartAt = startAt;
            var folderFileStartAt = new StartingPoint(X: folderStartAt.X + folderPadding, Y: folderStartAt.Y + folderPadding);

            foreach (var file in folder.Files)
            {
                var radius = (file.FileSize / minFileSize) * 10;
                var y = folderFileStartAt.Y + radius;
                var x = folderFileStartAt.X + radius;

                _stringBuilder.AddFileCircle(x, y, radius, file.Name);
                maxY = Math.Max(maxY, y + radius + folderPadding);
                folderFileStartAt = folderFileStartAt with { X = x + radius + filePadding };
            }

            var width = folderFileStartAt.X - folderStartAt.X;
            var height = maxY - folderStartAt.Y;
            var rectangleX = folderStartAt.X;
            var rectangleY = folderStartAt.Y;
            var folderName = folder.Name;
            _stringBuilder.AddBoundingRectangle(rectangleX, rectangleY, width, height, folderName);
        }

        return new ChartData { Data = _stringBuilder.ToSvgString() };
    }

    private record StartingPoint(long X, long Y);
}