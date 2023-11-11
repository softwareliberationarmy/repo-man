using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repo_man.domain.Git
{
    public static class FileExtensions
    {
        public static string GetFileExtension(this string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            return string.IsNullOrEmpty(fileExtension) ? fileName : fileExtension;
        }
    }
}
