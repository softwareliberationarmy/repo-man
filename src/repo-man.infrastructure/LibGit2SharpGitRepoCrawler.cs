using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using repo_man.domain.Git;

namespace repo_man.infrastructure
{
    public class LibGit2SharpGitRepoCrawler: IGitRepoCrawler
    {
        public IEnumerable<(string, long, Commit[])> GetFilesWithCommits()
        {
            throw new NotImplementedException();
        }
    }
}
