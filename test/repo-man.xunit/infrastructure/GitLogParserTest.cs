using System.IO;
using Microsoft.Extensions.Configuration;
using Moq;
using repo_man.domain.FileSystem;
using repo_man.infrastructure.FileSys;
using repo_man.infrastructure.Git;

namespace repo_man.xunit.infrastructure
{
    public class GitLogParserTest: TestBase
    {
        public GitLogParserTest()
        {
            _mocker.GetMock<IConfiguration>().Setup(c => c["repo"]).Returns("C:\\Temp\\MyRepo");
        }

        [Fact]
        public void CanParseSingleCommitLog()
        {
            _mocker.GetMock<IFileSystem>().Setup(x => x.GetFileSize("C:\\Temp\\MyRepo\\src\\Program.cs")).Returns(1L);
            var target = _mocker.CreateInstance<GitLogParser>();

            target.Parse("commit e026029a7e494fdcc54bc9d04efa40956f0d119a");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Tue Oct 22 10:55:03 2024 -0500");
            target.Parse("");
            target.Parse("    IPS-778 setting the userId regardless of whether the avatar and background are in local storage. (#253)");
            target.Parse("");
            target.Parse("M\tsrc/Program.cs");
            target.Parse("");

            var result = target.GetGitFileData().Single();

            result.Item1.Should().Be("src/Program.cs");
            result.Item2.Should().Be(1L);
            var commit = result.Item3.Single();
            commit.Hash.Should().Be("e026029a7e494fdcc54bc9d04efa40956f0d119a");
            commit.Author.Should().Be("kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            commit.CommitDate.Should().Be(new DateTimeOffset(2024, 10, 22, 10, 55, 3, TimeSpan.FromHours(-5)));
            commit.Message.Should().Be("IPS-778 setting the userId regardless of whether the avatar and background are in local storage. (#253)");
        }

        [Fact]
        public void IgnoresMergeLines()
        {
            _mocker.GetMock<IFileSystem>().Setup(x => x.GetFileSize("C:\\Temp\\MyRepo\\src\\Program.cs")).Returns(1L);
            var target = _mocker.CreateInstance<GitLogParser>();

            target.Parse("commit e026029a7e494fdcc54bc9d04efa40956f0d119a");
            target.Parse("Merge: 4efd5da13 26c06656f");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Tue Oct 22 10:55:03 2024 -0500");
            target.Parse("");
            target.Parse("    IPS-778 setting the userId regardless of whether the avatar and background are in local storage. (#253)");
            target.Parse("");
            target.Parse("M\tsrc/Program.cs");
            target.Parse("");

            var result = target.GetGitFileData().Single();

            result.Item1.Should().Be("src/Program.cs");
            result.Item2.Should().Be(1L);
            var commit = result.Item3.Single();
            commit.Hash.Should().Be("e026029a7e494fdcc54bc9d04efa40956f0d119a");
            commit.Author.Should().Be("kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            commit.CommitDate.Should().Be(new DateTimeOffset(2024, 10, 22, 10, 55, 3, TimeSpan.FromHours(-5)));
            commit.Message.Should().Be("IPS-778 setting the userId regardless of whether the avatar and background are in local storage. (#253)");
        }

        [Fact]
        public void MultipleCommentLines()
        {
            _mocker.GetMock<IFileSystem>().Setup(x => x.GetFileSize("C:\\Temp\\MyRepo\\src\\Program.cs")).Returns(1L);
            var target = _mocker.CreateInstance<GitLogParser>();

            target.Parse("commit e026029a7e494fdcc54bc9d04efa40956f0d119a");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Tue Oct 22 10:55:03 2024 -0500");
            target.Parse("");
            target.Parse("    IPS-778 setting the userId regardless of whether the avatar and background are in local storage. (#253)");
            target.Parse("");
            target.Parse("    * sub-comment here");
            target.Parse("");
            target.Parse("M\tsrc/Program.cs");
            target.Parse("");

            var result = target.GetGitFileData().Single();

            result.Item1.Should().Be("src/Program.cs");
            result.Item2.Should().Be(1L);
            var commit = result.Item3.Single();
            commit.Hash.Should().Be("e026029a7e494fdcc54bc9d04efa40956f0d119a");
            commit.Author.Should().Be("kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            commit.CommitDate.Should().Be(new DateTimeOffset(2024, 10, 22, 10, 55, 3, TimeSpan.FromHours(-5)));
            commit.Message.Should().Be("IPS-778 setting the userId regardless of whether the avatar and background are in local storage. (#253)\r\n* sub-comment here");
        }

        [Fact]
        public void TwoFilesModifiedInOneCommit()
        {
            _mocker.GetMock<IFileSystem>().Setup(x => x.GetFileSize(It.IsAny<string>())).Returns(1L);
            var target = _mocker.CreateInstance<GitLogParser>();

            target.Parse("commit e026029a7e494fdcc54bc9d04efa40956f0d119a");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Tue Oct 22 10:55:03 2024 -0500");
            target.Parse("");
            target.Parse("    IPS-778 setting the userId regardless of whether the avatar and background are in local storage. (#253)");
            target.Parse("");
            target.Parse("M\tsrc/Program.cs");
            target.Parse("M\tsrc/App.cs");
            target.Parse("");

            var result = target.GetGitFileData().ToArray();

            result[0].Item1.Should().Be("src/Program.cs");
            result[0].Item2.Should().Be(1L);
            var commit = result[0].Item3.Single();
            commit.Hash.Should().Be("e026029a7e494fdcc54bc9d04efa40956f0d119a");
            commit.Author.Should().Be("kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            commit.CommitDate.Should().Be(new DateTimeOffset(2024, 10, 22, 10, 55, 3, TimeSpan.FromHours(-5)));
            commit.Message.Should().Be("IPS-778 setting the userId regardless of whether the avatar and background are in local storage. (#253)");
            result[1].Item1.Should().Be("src/App.cs");
            result[1].Item2.Should().Be(1L);
            result[0].Item3.Single().Should().Be(commit);
        }

        [Fact]
        public void TwoLogEntriesModifyingOneFile()
        {
            _mocker.GetMock<IFileSystem>().Setup(x => x.GetFileSize("C:\\Temp\\MyRepo\\src\\Program.cs")).Returns(2L);
            var target = _mocker.CreateInstance<GitLogParser>();

            target.Parse("commit e026029a7e494fdcc54bc9d04efa40956f0d119a");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Tue Oct 22 10:55:03 2024 -0500");
            target.Parse("");
            target.Parse("    IPS-778 setting the userId regardless of whether the avatar and background are in local storage. (#253)");
            target.Parse("");
            target.Parse("M\tsrc/Program.cs");
            target.Parse("");
            target.Parse("commit 70ac6ff1ebe1390a5c4b19476e9132efd1ba8f25");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Mon Oct 21 09:44:53 2024 -0500");
            target.Parse("");
            target.Parse("    IPS-897 uncommenting staging and prod domains (#251)");
            target.Parse("");
            target.Parse("A\tsrc/Program.cs");
            target.Parse("");

            var result = target.GetGitFileData().Single();

            result.Item1.Should().Be("src/Program.cs");
            result.Item2.Should().Be(2L);
            var commits = result.Item3;
            commits.Length.Should().Be(2);
            commits.First().Hash.Should().Be("e026029a7e494fdcc54bc9d04efa40956f0d119a");
            commits.Last().Hash.Should().Be("70ac6ff1ebe1390a5c4b19476e9132efd1ba8f25");
        }

        [Fact]
        public void RenamedFilesAreTrackedCorrectly()
        {
            _mocker.GetMock<IFileSystem>().Setup(x => x.GetFileSize("C:\\Temp\\MyRepo\\src\\NewProgram.cs")).Returns(2L);
            var target = _mocker.CreateInstance<GitLogParser>();

            target.Parse("commit e026029a7e494fdcc54bc9d04efa40956f0d119a");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Tue Oct 22 10:55:03 2024 -0500");
            target.Parse("");
            target.Parse("    renaming file");
            target.Parse("");
            target.Parse("R100\tsrc/Program.cs\tsrc/NewProgram.cs");
            target.Parse("");
            target.Parse("commit 70ac6ff1ebe1390a5c4b19476e9132efd1ba8f25");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Mon Oct 21 09:44:53 2024 -0500");
            target.Parse("");
            target.Parse("    IPS-897 uncommenting staging and prod domains (#251)");
            target.Parse("");
            target.Parse("M\tsrc/Program.cs");
            target.Parse("");

            var result = target.GetGitFileData().Single();

            result.Item1.Should().Be("src/NewProgram.cs");
            result.Item2.Should().Be(2L);
            var commits = result.Item3;
            commits.Length.Should().Be(2);
            commits.First().Hash.Should().Be("e026029a7e494fdcc54bc9d04efa40956f0d119a");
            commits.Last().Hash.Should().Be("70ac6ff1ebe1390a5c4b19476e9132efd1ba8f25");
        }

        [Fact]
        public void MoreCommplicatedRenamingScenario()
        {
            _mocker.GetMock<IFileSystem>().Setup(x => x.GetFileSize("C:\\Temp\\MyRepo\\src\\NewProgram.cs")).Returns(2L);
            var target = _mocker.CreateInstance<GitLogParser>();

            target.Parse("commit e026029a7e494fdcc54bc9d04efa40956f0d119a");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Tue Oct 22 10:55:03 2024 -0500");
            target.Parse("");
            target.Parse("    renaming file");
            target.Parse("");
            target.Parse("R100\tsrc/Program.cs\tsrc/NewProgram.cs");
            target.Parse("");
            target.Parse("commit 70ac6ff1ebe1390a5c4b19476e9132efd1ba8f25");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Mon Oct 21 09:44:53 2024 -0500");
            target.Parse("");
            target.Parse("    IPS-897 uncommenting staging and prod domains (#251)");
            target.Parse("");
            target.Parse("M\tsrc/Program.cs");
            target.Parse("");
            target.Parse("commit e07bccbfccf95197026d5b43c97081341689762a");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Fri Oct 18 15:26:37 2024 -0500");
            target.Parse("");
            target.Parse("    IPS-897 uncommenting staging and prod domains (#251)");
            target.Parse("");
            target.Parse("R100\tsrc/NewProgram.cs\tsrc/Program.cs");
            target.Parse("");

            var result = target.GetGitFileData().Single();

            result.Item1.Should().Be("src/NewProgram.cs");
            var commits = result.Item3;
            commits.Length.Should().Be(3);
            commits.First().Hash.Should().Be("e026029a7e494fdcc54bc9d04efa40956f0d119a");
            commits.Last().Hash.Should().Be("e07bccbfccf95197026d5b43c97081341689762a");
        }

        [Fact]
        public void DeletedFilesArentReturned()
        {
            _mocker.GetMock<IFileSystem>().Setup(x => x.GetFileSize("C:\\Temp\\MyRepo\\src\\Program.cs")).Returns(0L);
            var target = _mocker.CreateInstance<GitLogParser>();

            target.Parse("commit e026029a7e494fdcc54bc9d04efa40956f0d119a");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Tue Oct 22 10:55:03 2024 -0500");
            target.Parse("");
            target.Parse("    IPS-778 setting the userId regardless of whether the avatar and background are in local storage. (#253)");
            target.Parse("");
            target.Parse("D\tsrc/Program.cs");
            target.Parse("");
            target.Parse("commit 70ac6ff1ebe1390a5c4b19476e9132efd1ba8f25");
            target.Parse("Author: kerry-patrick-il <146291855+kerry-patrick-il@users.noreply.github.com>");
            target.Parse("Date:   Mon Oct 21 09:44:53 2024 -0500");
            target.Parse("");
            target.Parse("    IPS-897 uncommenting staging and prod domains (#251)");
            target.Parse("");
            target.Parse("M\tsrc/Program.cs");
            target.Parse("");

            var result = target.GetGitFileData();
            result.Count().Should().Be(0);
        }
    }
}
