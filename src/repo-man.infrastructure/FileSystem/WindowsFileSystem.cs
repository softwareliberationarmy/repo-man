using repo_man.domain.FileSystem;

namespace repo_man.infrastructure.FileSys
{
    public class WindowsFileSystem: IFileSystem
    {
        public async Task WriteTextToFile(string fileContent, string filePath)
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
    }
}
