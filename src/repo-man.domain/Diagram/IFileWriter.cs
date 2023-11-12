namespace repo_man.domain.Diagram;

public interface IFileWriter
{
    Task WriteTextToFile(string fileContent, string filePath);
}