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
        var minFileSize = tree.GetMinFileSize();

        long x = 10;
        long startY = 0;
        long maxY = 0;
        for (int i = 0; i < tree.Files.Count; i++)
        {
            startY = 10;
            var file = tree.Files.ElementAt(i);
            var radius = (file.FileSize / minFileSize) * 10;
            x += radius;
            var y = startY + radius;

            _stringBuilder.AddFileCircle(x, y, radius, file.Name);
            x += radius;
            maxY = Math.Max(maxY, y + radius + 10);

        }

        startY = maxY;
        x = 15;

        for (int i = 0; i < tree.Folders.Count; i++)
        {
            startY += 10;
            var folder = tree.Folders.ElementAt(i);
            for (int j = 0; j < folder.Files.Count; j++)
            {
                var file = folder.Files.ElementAt(j);
                var radius = (file.FileSize / minFileSize) * 10;
                x += radius;
                long y = startY + 5 + radius;

                _stringBuilder.AddFileCircle(x, y, radius, file.Name);
                x += radius;
                maxY = Math.Max(maxY, y + radius + 10);
            }

            var width = x - 5;
            var height = maxY - 15;
            var rectangleX = 10;
            var rectangleY = startY;
            var folderName = folder.Name;
            _stringBuilder.AddBoundingRectangle(rectangleX, rectangleY, width, height, folderName);
        }

        return new ChartData { Data = _stringBuilder.ToSvgString() };
    }
}