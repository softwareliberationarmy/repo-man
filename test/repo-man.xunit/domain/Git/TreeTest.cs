using AutoFixture;
using FluentAssertions;
using repo_man.domain;
using repo_man.domain.Git;

namespace repo_man.xunit.domain.Git
{
    public class TreeTest
    {
        private readonly Fixture _fixture;

        public TreeTest()
        {
            _fixture = new Fixture();
        }

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
    }
}
