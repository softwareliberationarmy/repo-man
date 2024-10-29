namespace repo_man.domain.FileSystem;

public interface IFileSystem
{
    Task WriteTextToFileAsync(string fileContent, string filePath);

    long GetFileSize(string path);

    Task<string> ReadTextFromFileAsync(string filePath);
}