using AutoFixture;
using repo_man.domain;

namespace repo_man.xunit.domain.Git
{
    public class GitTreeTest: TestBase
    {
        [Fact]
        public void TopLevelFileSavedToTree()
        {
            //arrange
            var expectedHash = _fixture.Create<string>();
            var expectedDate = DateTimeOffset.Now;

            //act
            var target = new GitTree();
            target.AddFile("readme.md", _fixture.Create<long>(), new[] { new Commit(expectedHash, expectedDate, "Some Author", "Some Message") });

            //assert
            target.Files.Single().Commits.Single().Should()
                .BeEquivalentTo(new Commit(expectedHash, expectedDate, "Some Author", "Some Message"));
        }

        [Fact]
        public void FileFromTopLevelFolderStoredLowerInTree()
        {
            //arrange

            //act
            var target = new GitTree();
            target.AddFile(".config/dotnet-tools.json", _fixture.Create<long>(), new[] { new Commit(_fixture.Create<string>(), DateTimeOffset.Now, "Some Author", "Some Message") });

            target.Files.Should().BeEmpty();
            var folder = target.Folders.Single();
            folder.Name.Should().Be(".config");
            folder.Files.Single().Name.Should().Be("dotnet-tools.json");
        }

        [Fact]
        public void ItsTurtlesAllTheWayDown()
        {
            var target = new GitTree();
            target.AddFile("any/way/you/want/it/thats/the/way/you/need/it.txt", _fixture.Create<long>(), new[] { _fixture.Create<Commit>() });

            var expectedFolders = new[] { "any", "way", "you", "want", "it", "thats", "the", "way", "you", "need" };

            var currentFolders = target.Folders;
            for (int i = 0; i < expectedFolders.Length; i++)
            {
                currentFolders.Single().Name.Should().Be(expectedFolders[i]);
                currentFolders = currentFolders.Single().Folders;
            }
        }

        [Fact]
        public void StoresTheFullFilePath()
        {
            var target = new GitTree();
            target.AddFile("src/main/file1.cs", 1000, []);
            target.Folders.Single().Folders.Single().Files.Single().FullPath.Should().Be("src/main/file1.cs");
        }

        [Fact]
        public void RemembersTheSmallestFileSize()
        {
            var expectedSize = 1024;
            var target = new GitTree();
            target.AddFile("Program.cs", expectedSize * 3, Array.Empty<Commit>());
            target.AddFile("src/ImportantFile.cs", expectedSize, Array.Empty<Commit>());
            target.AddFile("test/Project.Test/MyTestFile.cs", expectedSize * 2, Array.Empty<Commit>());

            target.GetMinFileSize().Should().Be(expectedSize);
        }

        [Fact]
        public void RemembersTheLargestFileSize()
        {
            var expectedSize = 1024;
            var target = new GitTree();
            target.AddFile("Program.cs", expectedSize / 3, Array.Empty<Commit>());
            target.AddFile("src/ImportantFile.cs", expectedSize, Array.Empty<Commit>());
            target.AddFile("test/Project.Test/MyTestFile.cs", expectedSize / 2, Array.Empty<Commit>());

            target.GetMaxFileSize().Should().Be(expectedSize);
        }

        [Fact]
        public void RemembersTheSmallestCommitCount()
        {
            var target = new GitTree();
            target.AddFile("Program.cs", 500, new Commit[]{ new Commit("A")});
            target.AddFile("src/ImportantFile.cs", 500, new Commit[] { new Commit("A"), new Commit("B") });
            target.AddFile("test/Project.Test/MyTestFile.cs", 500, new Commit[] { new Commit("A"),  new Commit("B") });

            target.GetMinCommitCount().Should().Be(1);
        }

        [Fact]
        public void RemembersTheLargestCommitCount()
        {
            var target = new GitTree();
            target.AddFile("Program.cs", 500, new Commit[] { new Commit("A") });
            target.AddFile("src/ImportantFile.cs", 500, new Commit[] { new Commit("A"), new Commit("B") });
            target.AddFile("test/Project.Test/MyTestFile.cs", 500, new Commit[] { new Commit("A"), new Commit("B") });

            target.GetMaxCommitCount().Should().Be(2);
        }

        [Fact]
        public void GetAllFilesDoesTheRecursingForYou()
        {
            var tree = new GitTree();
            tree.AddFile("src/main/file1.txt", 10, []);
            tree.AddFile("src/main/sub/file1.txt", 10, []);
            tree.AddFile("src/main/sub/sub/sub/file1.txt", 10, []);

            tree.GetAllFiles().Count().Should().Be(3);
        }
    }
}
