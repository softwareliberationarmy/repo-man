using System.Drawing;
using System.Text;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram;

public class SvgLegendDataWriter
{
    private readonly IFileColorMapper _colorMapper;

    public SvgLegendDataWriter(IFileColorMapper colorMapper)
    {
        _colorMapper = colorMapper;
    }

    public LegendData WriteLegendData(GitTree tree, Point startingPoint)
    {
        var builder = new StringBuilder();
        builder.Append($"<g transform=\"translate({startingPoint.X}, {startingPoint.Y})\">");

        var extensions = new SortedSet<string>();
        foreach (var file in tree.Files)
        {
            CountFileExtension(file, extensions);
        }

        InspectFolders(tree.Folders, extensions);

        var y = 0;
        foreach (var extension in extensions)
        {
            builder.Append($"<g transform=\"translate(0, {y})\">");
            builder.Append($"<circle r=\"5\" fill=\"{_colorMapper.Map(extension)}\"></circle>");
            builder.Append(
                $"<text x=\"10\" style=\"font-size:14px;font-weight:300\" dominant-baseline=\"middle\">{extension}</text>");
            builder.Append("</g>");
            y += 15;
        }
        builder.Append("</g>");

        return new LegendData
        {
            Data = builder.ToString()
        };
    }

    private static void InspectFolders(IReadOnlyCollection<GitFolder> readOnlyCollection, SortedSet<string> extensions)
    {
        foreach (var folder in readOnlyCollection)
        {
            foreach (var file in folder.Files)
            {
                CountFileExtension(file, extensions);
            }
            InspectFolders(folder.Folders, extensions);
        }
    }

    private static void CountFileExtension(GitFile file, SortedSet<string> extensions)
    {
        extensions.Add(file.Name.GetFileExtension());
    }

}