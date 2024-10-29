using System.Reflection.Metadata.Ecma335;
using repo_man.domain.FileSystem;

namespace repo_man.infrastructure.FileSys
{
    public class WindowsFileSystem: IFileSystem
    {
        public async Task WriteTextToFileAsync(string fileContent, string filePath)
        {
            await File.WriteAllTextAsync(filePath, fileContent);
        }

        public long GetFileSize(string path)
        {
            if (File.Exists(path))
            {
                return new FileInfo(path).Length;
            }

            return 0;
        }

        public async Task<string> ReadTextFromFileAsync(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }
    }
}
