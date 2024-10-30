namespace repo_man.domain.Diagram.FileColorMapper;

public interface IFileColorMapper
{
    string Map(string fileExtension, byte intensity = 100);
}