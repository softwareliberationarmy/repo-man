using repo_man.domain.Git;

namespace repo_man.domain.AI;

public interface IRagDataBuilder
{
    string CreateCommitData(IEnumerable<(string, long, Commit[])> commitHistory);
}