using AutoFixture;
using FluentAssertions;
using repo_man.domain;
using repo_man.domain.Git;
using repo_man.xunit._helpers;

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
            target.AddFile("readme.md", _fixture.Create<long>(), new[] { new Commit(expectedHash, expectedDate) });

            //assert
            target.Files.Single().Commits.Single().Should()
                .BeEquivalentTo(new Commit(expectedHash, expectedDate));
        }

        [Fact]
        public void FileFromTopLevelFolderStoredLowerInTree()
        {
            //arrange

            //act
            var target = new GitTree();
            target.AddFile(".config/dotnet-tools.json", _fixture.Create<long>(), new[] { new Commit(_fixture.Create<string>(), DateTimeOffset.Now) });

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
    }
}
