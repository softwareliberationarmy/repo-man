using repo_man.domain.Git;

namespace repo_man.domain.AI
{
    public class JsonRagDataBuilder: IRagDataBuilder
    {
        public string CreateCommitData(IEnumerable<(string, long, Commit[])> commitHistory)
        {
            throw new NotImplementedException();
        }
    }
}
