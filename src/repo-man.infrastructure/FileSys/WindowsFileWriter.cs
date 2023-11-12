using repo_man.domain.Diagram;

namespace repo_man.infrastructure.FileSys
{
    public class WindowsFileWriter: IFileWriter
    {
        public async Task WriteTextToFile(string fileContent, string filePath)
        {
            await File.WriteAllTextAsync(filePath, fileContent);
        }
    }
}
