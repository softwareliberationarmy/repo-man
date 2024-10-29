using repo_man.domain.AI;

namespace repo_man.xunit.domain.AI
{
    public class JsonRagDataBuilderTest: TestBase
    {
        [Fact]
        public void ItBuildsTheJsonDataFromTheCommitHistory()
        {
            var commitHistory = new List<(string, long, Commit[])>
            {
                ("src/Folder1/File1.cs", 12345L, [new("hash1"){ Message = "Here is a message."}]),
                ("src/Folder2/File2.cs", 54321L, [new("hash2"){ Message = "Here is a two-line message.\r\nThis is line two."}])
            };
            var target = _mocker.CreateInstance<JsonRagDataBuilder>();
            var result = target.CreateCommitData(commitHistory);
            result.Should().Be("[{\"name\":\"src/Folder1/File1.cs\",\"size\":12345,\"commits\":[{\"hash\":\"hash1\",\"message\":\"Here is a message.\"}]},{\"name\":\"src/Folder2/File2.cs\",\"size\":54321,\"commits\":[{\"hash\":\"hash2\",\"message\":\"Here is a two-line message.\"}]}]");
        }
    }
}
