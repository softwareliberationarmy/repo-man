namespace repo_man.infrastructure.FileSys
{
    public class WindowsFileSize
    {
        public virtual long GetSize(string path)
        {
            if (File.Exists(path))
            {
                return new FileInfo(path).Length;
            }

            return 0;
        }
    }
}
