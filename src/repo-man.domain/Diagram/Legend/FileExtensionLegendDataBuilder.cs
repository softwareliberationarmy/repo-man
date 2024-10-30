using repo_man.domain.Diagram.FileColorMapper;
using repo_man.domain.Git;

namespace repo_man.domain.Diagram.Legend;

public class FileExtensionLegendDataBuilder
{
    private readonly IFileColorMapper _colorMapper;

    public FileExtensionLegendDataBuilder(IFileColorMapper colorMapper)
    {
        _colorMapper = colorMapper;
    }

    public virtual Dictionary<string, string> BuildLegendOptions(GitTree tree)
    {
        var extensions = new SortedSet<string>();
        foreach (var file in tree.Files)
        {
            CountFileExtension(file, extensions);
        }

        InspectFolders(tree.Folders, extensions);

        var newExtensions = new Dictionary<string, string>();
        foreach (var extension in extensions)
        {
            newExtensions.Add(extension, _colorMapper.Map(extension));
        }

        return newExtensions;
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