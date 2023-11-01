using System.Text;

namespace repo_man.domain.Diagram;

public class SvgStringBuilder
{
    private readonly IFileColorMapper _colorMapper;
    private readonly StringBuilder _builder = new StringBuilder();

    public SvgStringBuilder(IFileColorMapper colorMapper)
    {
        _colorMapper = colorMapper;
    }

    public void AddBoundingRectangle(int rectangleX, long rectangleY, long width,
        long height, string folderName)
    {
        _builder.Append($"<g transform=\"translate({rectangleX},{rectangleY})\">");
        _builder.Append(
            $"<rect fill=\"none\" stroke-width=\"0.5\" stroke=\"black\" width=\"{width}\" height=\"{height}\" />");
        _builder.Append($"<text style=\"fill:black\" font-size=\"6\" transform=\"translate(-1,-1)\" >{folderName}</text>");
        _builder.Append("</g>");
    }

    public void AddFileCircle(long x, long y, long radius, string fileName)
    {
        var color = _colorMapper.Map(Path.GetExtension(fileName));

        _builder.Append($"<g style=\"fill:{color}\" transform=\"translate({x},{y})\">");
        _builder.Append($"<circle r=\"{radius}\" />");
        _builder.Append(
            $"<text style=\"fill:black\" font-size=\"6\" alignment-baseline=\"middle\" text-anchor=\"middle\"/>{fileName}</text>");
        _builder.Append("</g>");
    }

    public string ToSvgString()
    {
        return _builder.ToString();
    }
}