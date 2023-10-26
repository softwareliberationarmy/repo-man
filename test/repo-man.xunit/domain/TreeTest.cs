using AutoFixture;
using FluentAssertions;
using repo_man.domain;
using repo_man.domain.Git;

namespace repo_man.xunit.domain
{
    public class TreeTest
    {
        [Fact]
        public void TopLevelFileSavedToTree()
        {
            //arrange
            var fixture = new Fixture();
            var expectedHash = fixture.Create<string>();
            var expectedDate = DateTimeOffset.Now;

            //act
            var target = new Tree();
            target.AddFile("readme.md", fixture.Create<long>(),  new[] { new Commit(expectedHash, expectedDate) });

            //assert
            target.TopLevelFiles.Single().Commits.Single().Should()
                .BeEquivalentTo(new Commit(expectedHash, expectedDate));
        }

        [Fact]
        public void FileFromTopLevelFolderStoredLowerInTree()
        {
            //arrange
            var fixture = new Fixture();

            //act
            var target = new Tree();
            target.AddFile(".config/dotnet-tools.json", fixture.Create<long>(), new []{ new Commit(fixture.Create<string>(), DateTimeOffset.Now)});

            target.TopLevelFiles.Should().BeEmpty();
            var folder = target.Folders.Single();
            folder.Name.Should().Be(".config");
            folder.Files.Single().Name.Should().Be("dotnet-tools.json");
        }
    }
}
