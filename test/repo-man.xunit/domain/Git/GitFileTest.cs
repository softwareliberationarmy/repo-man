using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repo_man.xunit.domain.Git
{
    public class GitFileTest: TestBase
    {
        [Fact]
        public void SetsToolTipByDefaultWhenGitFileCreated()
        {
            var gitFile = new GitFile("test.txt", 100, new List<Commit>(), "src/files/test.txt");
            gitFile.ToolTip.Should().Be("src/files/test.txt\r\n\r\nFile size (bytes): 100\r\n# of commits: 0");
        }

        [Fact]
        public void SetsLabelByDefaultWhenGitFileCreated()
        {
            var gitFile = new GitFile("test.txt", 100, new List<Commit>(), "src/files/test.txt");
            gitFile.Label.Should().Be("test.txt");
        }
    }
}
