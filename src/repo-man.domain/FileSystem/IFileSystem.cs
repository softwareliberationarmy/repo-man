namespace repo_man.domain.FileSystem;

public interface IFileSystem
{
    Task WriteTextToFile(string fileContent, string filePath);

    long GetFileSize(string path);
}