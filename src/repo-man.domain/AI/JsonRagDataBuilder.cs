using System.Text.Json;
using System.Text.Json.Nodes;
using repo_man.domain.Git;

namespace repo_man.domain.AI
{
    public class JsonRagDataBuilder: IRagDataBuilder
    {
        public string CreateCommitData(IEnumerable<(string, long, Commit[])> commitHistory)
        {
            var files = commitHistory.Select(x => new
            {
                name = x.Item1,
                size = x.Item2,
                commits = x.Item3.Select(y => new
                {
                    hash = y.Hash
                })
            });
            return JsonSerializer.Serialize(files);
        }
    }
}
